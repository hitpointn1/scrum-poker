using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Models;

namespace PlanningPoker.Controllers
{
    public class ScrumRoomController : Controller
    {

        IHubContext<VotingHub> hubContext;

        public ScrumRoomController(IHubContext<VotingHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        enum CharTypeCards
        {
            XS = 1,
            S,
            M,
            L,
            XL,
            XXL
        }

        //Вычисление оценки задачи
        private string ResultMarks(List<string> result, int TypeCards)
        {
            double sum = 0;
            int count = 0;
            int card;

            int resultcard;
            switch(TypeCards)
            {
                case 1:
                    foreach(var res in result)
                    {
                        if (int.TryParse(res, out card))
                        {
                            sum += card;
                            count++;
                        }
                    }
                    if (count == 0)
                        resultcard = 0;
                    else
                        resultcard = (int) Math.Ceiling(sum / count);

                    if (resultcard < 1)
                        return "-";
                    else if (resultcard < 2)
                        return "1";
                    else if (resultcard < 3)
                        return "2";
                    else if (resultcard < 5)
                        return "3";
                    else if (resultcard < 8)
                        return "5";
                    else if (resultcard < 13)
                        return "8";
                    else if (resultcard < 20)
                        return "13";
                    else if (resultcard < 40)
                        return "20";
                    else if (resultcard < 100)
                        return "40";
                    else
                        return "100";
                case 2:
                    foreach (var res in result)
                    {
                        if (int.TryParse(res, out card))
                        {
                            sum += card;
                            count++;
                        }
                    }
                    if (count == 0)
                        resultcard = 0;
                    else
                        resultcard = (int)Math.Ceiling(sum / count);

                    if (resultcard < 1)
                        return "-";
                    else if (resultcard < 2)
                        return "1";
                    else if (resultcard < 3)
                        return "2";
                    else if (resultcard < 5)
                        return "3";
                    else if (resultcard < 8)
                        return "5";
                    else if (resultcard < 13)
                        return "8";
                    else if (resultcard < 20)
                        return "13";
                    else if (resultcard < 40)
                        return "20";
                    else
                        return "40";

                case 3:
                    foreach (var res in result)
                    {
                        switch (res)
                        {
                            case "XS":
                                sum += (int)CharTypeCards.XS;
                                count++;
                                break;
                            case "S":
                                sum += (int)CharTypeCards.S;
                                count++;
                                break;
                            case "M":
                                sum += (int)CharTypeCards.M;
                                count++;
                                break;
                            case "L":
                                sum += (int)CharTypeCards.L;
                                count++;
                                break;
                            case "XL":
                                sum += (int)CharTypeCards.XL;
                                count++;
                                break;
                            case "XXL":
                                sum += (int)CharTypeCards.XXL;
                                count++;
                                break;
                            default:
                                break;
                        }

                    }
                    if (count == 0)
                        resultcard = 0;
                    else
                        resultcard = (int)Math.Ceiling(sum / count);

                    if (resultcard < 1)
                        return "-";
                    else if (resultcard < 2)
                        return "XS";
                    else if (resultcard < 3)
                        return "S";
                    else if (resultcard < 4)
                        return "M";
                    else if (resultcard < 5)
                        return "L";
                    else if (resultcard < 6)
                        return "XL";
                    else
                        return "XXL";
                case 4:
                    foreach (var res in result)
                    {
                        if (int.TryParse(res, out card))
                        {
                            sum += card;
                            count++;
                        }
                    }
                    if (count == 0)
                        resultcard = 0;
                    else
                        resultcard = (int)Math.Ceiling(sum / count);

                    if (resultcard < 1)
                        return "-";
                    else if (resultcard < 2)
                        return "1";
                    else if (resultcard < 5)
                        return "2";
                    else if (resultcard < 10)
                        return "5";
                    else if (resultcard < 20)
                        return "10";
                    else if (resultcard < 50)
                        return "20";
                    else if (resultcard < 100)
                        return "50";
                    else 
                        return "100";
                default:
                    return "Ошибка";
            }
        }

        [HttpGet]
        // Вход в комнату для создателя комнаты.
        public IActionResult RoomEntrance(int PokerRoomId, int PlayerId)
        {
            ViewBag.PokerRoomId = PokerRoomId;
            using (var _context = new PokerPlanningContext())
            {
                var player = _context.Players.Where(p => p.Id == PlayerId).SingleOrDefault();

                try
                {
                    player.IsOnline = true;
                    _context.Players.Update(player);
                    _context.SaveChanges();
                }
                catch (NullReferenceException)
                {
                    return new BadRequestResult();
                }

                List<Topic> topics = _context.Topics.Where(t => t.PokerRoomId == PokerRoomId).ToList();

                ViewBag.NamePlayer = player.Name;
                ViewBag.PlayerId = player.Id;

                return View(topics);
            }
        }

        [HttpPost]
        // Создание задачи для голосовани.
        public IActionResult TopicCreate(int PokerRoomId, int PlayerId, string Title, string Description)
        {
            try
            {
                using (var _context = new PokerPlanningContext())
                {
                    var Topic = new Topic
                    {
                        Title = Title,
                        Description = Description,
                        Status = 1, // не обсуждается
                        PokerRoomId = PokerRoomId, 
                        IterationNumb = 0
                    };

                    _context.Topics.Add(Topic);
                    _context.SaveChanges();
                }
            }
            catch (ArgumentNullException)
            {
                return new BadRequestResult();//Сделать: Отправка в форму, что не введено;
            }
            return RedirectToAction("RoomEntrance", new { PokerRoomId, PlayerId });
        }

        [HttpPost]
        // Старт или стоп обсуждения задачи.
        public IActionResult StartVoting(int PokerRoomId, int PlayerId, int IdTopic)
        {
            try
            {
                using (var _context = new PokerPlanningContext())
                {
                    var Topic = new Topic();
                    Topic = _context.Topics.Where(t => t.Id == IdTopic).SingleOrDefault();

                    var countTopicStart = _context.Topics.Where(t => t.PokerRoomId == PokerRoomId && t.Status == 2)
                        .ToList().Count;

                    if (Topic.Status == 1 && countTopicStart == 0)//если голосование необходимо запустить
                    {
                        Topic.Status = 2; // старт голосования
                        Topic.IterationNumb++; 
                    }
                    else if (Topic.Status == 2 && countTopicStart == 1)
                    {
                        Topic.Status = 1; // стоп голосования

                        var cards = (List<string>) _context.Cards.Where(c =>c.TopicId == IdTopic && c.IterationNumb == Topic.IterationNumb)
                            .Select(s=> s.CardValue).ToList(); // получение всех карточек для данного топика

                        var typeCards = _context.PokerRooms.Where(t => t.Id == PokerRoomId).
                            SingleOrDefault().TypeCards; // получение типа карточек

                        Topic.Marks = ResultMarks(cards, typeCards); // подсчет общей оценки для карточки
                    }
                    else
                    {
                        //предупреждение
                    }
                    _context.Topics.Update(Topic);
                    _context.SaveChanges();
                }
            }
            catch (ArgumentNullException)
            {
                return new BadRequestResult();//Сделать: Отправка в форму, что не введено;
            }
            return RedirectToAction("RoomEntrance", new { PokerRoomId, PlayerId });
        }

        [HttpGet]
        //Комната обсуждения задачи.
        public IActionResult RoomDiscussion(int PokerRoomId, int PlayerId) //SignalR настроить
        {
            ViewBag.PokerRoomId = PokerRoomId;

            List<string> valueCards = new List<string>();

            using (var _context = new PokerPlanningContext())
            {
                var player = _context.Players.Where(p => p.Id == PlayerId).SingleOrDefault();

                var typeCards = _context.PokerRooms.Where(p => p.Id == PokerRoomId).SingleOrDefault().TypeCards;

                string stringValueCards = "";

                switch (typeCards)
                {
                    case 1:
                        stringValueCards = "1,2,3,5,8,13,20,40,100,∞,?,CC";
                        break;
                    case 2:
                        stringValueCards = "1,2,3,5,8,13,20,40,?";
                        break;
                    case 3:
                        stringValueCards = "XS,S,M,L,XL,XXL,?";
                        break;
                    case 4:
                        stringValueCards = "1,2,5,10,20,50,100";
                        break;
                }


                valueCards = stringValueCards.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                var TopicDiscussion = _context.Topics.Where(t => t.PokerRoomId == PokerRoomId && t.Status == 2);
                var countTopicDiscussion = TopicDiscussion.ToList().Count;

                ViewBag.CountTopicDiscussion = countTopicDiscussion;
                //Если тема обсуждается.
                if (countTopicDiscussion != 0)
                {
                    ViewBag.Title = TopicDiscussion.SingleOrDefault().Title;
                    ViewBag.Description = TopicDiscussion.SingleOrDefault().Description;
                    ViewBag.IdTopic = TopicDiscussion.SingleOrDefault().Id;
                }

                try
                {
                    player.IsOnline = true;
                    _context.Players.Update(player);
                    _context.SaveChanges();
                }
                catch (NullReferenceException)
                {
                    return new BadRequestResult();
                }


                ViewBag.NamePlayer = player.Name;
                ViewBag.PlayerId = player.Id;

                return View(valueCards);
            }
        }

        [HttpPost]
        //Удаление комнаты.
        public IActionResult DeletePokerRoom(int PokerRoomId)
        {
            using (var _context = new PokerPlanningContext())
            {
                // Начало транзакции.
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        //Удаление сообщений для комнаты.
                        var messages = _context.Messages.Where(m => m.PokerRoomId == PokerRoomId);
                        _context.Messages.RemoveRange(messages);
                        _context.SaveChanges();

                        //Удаление карточек и задач для комнаты.
                        var topics = _context.Topics.Where(t => t.PokerRoomId == PokerRoomId);
                        foreach (var topic in topics)
                        {
                            var cards = _context.Cards.Where(c => c.TopicId == topic.Id);
                            _context.Cards.RemoveRange(cards);
                            _context.Topics.Remove(topic);
                            _context.SaveChanges();
                        }

                        // Удаление всех игроков для комнаты.
                        var players = _context.Players.Where(p => p.PokerRoomId == PokerRoomId);
                        _context.Players.RemoveRange(players);
                        _context.SaveChanges();

                        // Удаление комнаты.
                        var pokerRoom = _context.PokerRooms.Where(p => p.Id == PokerRoomId).Single();
                        _context.PokerRooms.Remove(pokerRoom);
                        _context.SaveChanges();

                        transaction.Commit();
                    }
                    catch(Exception)
                    {
                        return new BadRequestResult();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
  
        }

        public async Task<IActionResult> GetResultVote(int PokerRoomId, int PlayerId, string ValueCard, int TopicId, string Comment)
        {
            using (var _context = new PokerPlanningContext())
            {
                // Вычисление итерации голосования топика. 
                var count = _context.Topics.Where(t => t.Id == TopicId).SingleOrDefault().IterationNumb;

                var cardIteration = _context.Cards.Where(c => c.TopicId == TopicId && c.PlayerId == PlayerId && c.IterationNumb == count)
                    .SingleOrDefault(); // Проверка, существует ли карточка в БД для этого игрока.

                if (cardIteration == null)
                {
                    var Cards = new Card
                    {
                        CardValue = ValueCard,
                        PlayerId = PlayerId,
                        TopicId = TopicId,
                        Comment = Comment,
                        IterationNumb = count
                    };

                    _context.Cards.Add(Cards);
                    _context.SaveChanges();
                }
                else
                {
                    //предупреждение
                }
            }
          //  await hubContext.Clients.All.SendAsync("Notify", $"Добавлено: {ValueCard}");
            return RedirectToAction("RoomDiscussion", "ScrumRoom", new { PokerRoomId, PlayerId });
        }
    }
}