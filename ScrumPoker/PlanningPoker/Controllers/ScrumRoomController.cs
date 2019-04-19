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

        private string ConvertToString(int number)
        {
            string str = "-";
            switch (number)
            {
                case 1: str = "XS"; break;
                case 2: str = "S"; break;
                case 3: str = "M"; break;
                case 4: str = "L"; break;
                case 5: str = "XL"; break;
                case 6: str = "XXL"; break;
            }
            return str;
        }

        //Вычисление оценки задачи.
        private string ResultMarks(List<string> result, int TypeCards)
        {
            int min = 100;
            int max = 0;
            int card;

            switch(TypeCards)
            {
                case 1:
                    foreach(var res in result)
                    {
                        if (int.TryParse(res, out card))
                        {
                            if (min > card)
                                min = card;
                            if (max < card)
                                max = card;
                        }
                    }
                    if (min == 100 && max == 0)
                        return "-";
                    else if (min == max)
                        return $"{min}";
                    else
                        return $"min = {min}; max = {max}";

                case 2:
                    foreach (var res in result)
                    {
                        if (int.TryParse(res, out card))
                        {
                            if (min > card)
                                min = card;
                            if (max < card)
                                max = card;
                        }
                    }
                    if (min == 100 && max == 0)
                        return "-";
                    else if (min == max)
                        return $"{min}";
                    else
                        return $"min = {min}; max = {max}";

                case 3:
                    min = 7;
                    max = 0;
                    foreach (var res in result)
                    {
                        switch (res)
                        {
                            case "XS":
                                if (min > (int)CharTypeCards.XS)
                                    min = (int)CharTypeCards.XS;
                                if (max < (int)CharTypeCards.XS)
                                    max = (int)CharTypeCards.XS;
                                break;
                            case "S":
                                if (min > (int)CharTypeCards.S)
                                    min = (int)CharTypeCards.S;
                                if (max < (int)CharTypeCards.S)
                                    max = (int)CharTypeCards.S;
                                break;
                            case "M":
                                if (min > (int)CharTypeCards.M)
                                    min = (int)CharTypeCards.M;
                                if (max < (int)CharTypeCards.M)
                                    max = (int)CharTypeCards.M;
                                break;
                            case "L":
                                if (min > (int)CharTypeCards.L)
                                    min = (int)CharTypeCards.L;
                                if (max < (int)CharTypeCards.L)
                                    max = (int)CharTypeCards.L;
                                break;
                            case "XL":
                                if (min > (int)CharTypeCards.XL)
                                    min = (int)CharTypeCards.XL;
                                if (max < (int)CharTypeCards.XL)
                                    max = (int)CharTypeCards.XL;
                                break;
                            case "XXL":
                                if (min > (int)CharTypeCards.XXL)
                                    min = (int)CharTypeCards.XXL;
                                if (max < (int)CharTypeCards.XXL)
                                    max = (int)CharTypeCards.XXL;
                                break;
                            default:
                                break;
                        }
                    }
                    if (min == 7 && max == 0)
                    {
                        return "-";
                    }
                    else if (min == max)
                    {
                        return $"{ConvertToString(min)}";
                    }
                    else
                    {
                        return $"min = {ConvertToString(min)}; max = {ConvertToString(max)}";
                    }
                    

                case 4:
                    foreach (var res in result)
                    {
                        if (int.TryParse(res, out card))
                        {
                            if (min > card)
                                min = card;
                            if (max < card)
                                max = card;
                        }
                    }
                    if (min == 100 && max == 0)
                        return "-";
                    else if (min == max)
                        return $"{min}";
                    else
                        return $"min = {min}; max = {max}";

                default:
                    return "Ошибка";
            }
        }

        [HttpGet]
        // Вход в комнату для создателя комнаты.
        public IActionResult RoomEntrance(int PokerRoomId, int PlayerId, int Password)
        {
            //Результат карточек.

            Dictionary<string, (string, string)> resultCard = new Dictionary<string, (string, string)>();

            ViewBag.PokerRoomId = PokerRoomId;

            using (var _context = new PokerPlanningContext())
            {
                var pw = _context.PokerRooms.FirstOrDefault(rid => rid.Id == PokerRoomId).Password;
                if (Equals(Password, pw))
                {
                    ViewBag.Password = pw;
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

                    // Выбор карточек для отображения результатов голосования.
                    var Topic = _context.Topics.Where(t => t.PokerRoomId == PokerRoomId && t.Status == 2).SingleOrDefault();
                    if (Topic != null)
                    {
                        var cards = _context.Cards.Where(c => c.TopicId == Topic.Id && c.IterationNumb == Topic.IterationNumb).ToList();
                        if (cards != null)
                        {
                            foreach (var card in cards)
                            {
                                var playerName = _context.Players.Where(p => p.Id == card.PlayerId).SingleOrDefault().Name;
                                resultCard[playerName] = (card.CardValue, card.Comment);
                            }
                        }
                    }

                    //Список пользователей-онлайн.
                    var playersOnline = _context.Players.Where(p => p.PokerRoomId == PokerRoomId && p.IsOnline == true).ToList();

                    ViewBag.NamePlayer = player.Name;
                    ViewBag.PlayerId = player.Id;

                    return View((topics, resultCard, playersOnline));
                }
                else
                    return RedirectToAction("RoomsList", "Home");
            }
        }

        [HttpPost]
        // Создание задачи для голосовани.
        public IActionResult TopicCreate(int PokerRoomId, int PlayerId, string Title, string Description, int password)
        {
            if (Title != null)
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
            }
            return RedirectToAction("RoomEntrance", new { PokerRoomId, PlayerId, password });
        }

        [HttpPost]
        // Старт или стоп обсуждения задачи.
        public async Task<IActionResult> StartVoting(int PokerRoomId, int PlayerId, int IdTopic,int password)
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

                        var cards = (List<string>)_context.Cards.Where(c => c.TopicId == IdTopic && c.IterationNumb == Topic.IterationNumb)
                            .Select(s => s.CardValue).ToList(); // получение всех карточек для данного топика

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
            await hubContext.Clients.All.SendAsync("UpdatePage");
            return RedirectToAction("RoomEntrance", new { PokerRoomId, PlayerId, password });
        }

        [HttpGet]
        //Комната обсуждения задачи.
        public IActionResult RoomDiscussion(int PokerRoomId, int PlayerId , int password) //SignalR настроить
        {
            ViewBag.PokerRoomId = PokerRoomId;

            List<string> valueCards = new List<string>();

            using (var _context = new PokerPlanningContext())
            {
                var pw = _context.PokerRooms.FirstOrDefault(rid => rid.Id == PokerRoomId).Password;
                if (Equals(password, pw))
                {
                    ViewBag.Password = pw;
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
                        var topic = TopicDiscussion.SingleOrDefault();

                        var cards = _context.Cards.Where(c => c.TopicId == topic.Id
                            && c.IterationNumb == topic.IterationNumb && c.PlayerId == PlayerId).SingleOrDefault();

                        if (cards == null)
                        {
                            ViewBag.StatusMessage = 0; // еще нет карточки
                        }
                        else
                        {
                            ViewBag.StatusMessage = 1; // уже обсудили
                            ViewBag.Value = cards.CardValue;
                        }
                        ViewBag.Title = topic.Title;
                        ViewBag.Description = topic.Description;
                        ViewBag.IdTopic = topic.Id;
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

                    //Список пользователей-онлайн.
                    var playersOnline = _context.Players.Where(p => p.PokerRoomId == PokerRoomId && p.IsOnline == true).ToList();

                    ViewBag.NamePlayer = player.Name;
                    ViewBag.PlayerId = player.Id;

                    return View((valueCards, playersOnline));
                }
                return RedirectToAction("RoomsList", "Home");

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

        //Получение результатов голосования участников.
        public async Task<IActionResult> GetResultVote(int PokerRoomId, int PlayerId, string ValueCard, int TopicId, string Comment, int password)
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
            await hubContext.Clients.All.SendAsync("UpdatePage");
            return RedirectToAction("RoomDiscussion", "ScrumRoom", new { PokerRoomId, PlayerId, password });
        }

        [HttpGet]
        //Редактирование задачи.
        public IActionResult EditTopic(int id)
        {
            Topic topic;
            using (var _context = new PokerPlanningContext())
            {
                topic = _context.Topics.Where(t => t.Id == id).SingleOrDefault();

                ViewBag.Password = _context.PokerRooms.FirstOrDefault(rid => rid.Id == topic.PokerRoomId).Password;

                ViewBag.PokerRoomId = topic.PokerRoomId;
   
                var player = _context.Players.Where(p => p.PokerRoomId == topic.PokerRoomId && p.Role == 2).SingleOrDefault();
                ViewBag.PlayerId = player.Id;
            }
            return View(topic);
        }


        [HttpPost]
        public IActionResult EditTopic(Topic topic)
        {
            if (ModelState.IsValid)
            {
                using (var _context = new PokerPlanningContext())
                {
                    _context.Topics.Update(topic);
                    _context.SaveChanges();

                    ViewBag.PokerRoomId = topic.PokerRoomId;
                    ViewBag.Password = _context.PokerRooms.FirstOrDefault(rid => rid.Id == topic.PokerRoomId).Password;
                    var player = _context.Players.Where(p => p.PokerRoomId == topic.PokerRoomId && p.Role == 2).SingleOrDefault();
                    ViewBag.PlayerId = player.Id;
                    
                }
            }
            return View(topic);
        }

        [HttpGet]
        //Удаление задачи.
        public IActionResult DeleteTopic(int id)
        {
            Topic topic;
            using (var _context = new PokerPlanningContext())
            {
                topic = _context.Topics.Where(t => t.Id == id).SingleOrDefault();

                ViewBag.Password = _context.PokerRooms.FirstOrDefault(rid => rid.Id == topic.PokerRoomId).Password;

                ViewBag.PokerRoomId = topic.PokerRoomId;
                var player = _context.Players.Where(p => p.PokerRoomId == topic.PokerRoomId && p.Role == 2).SingleOrDefault();
                ViewBag.PlayerId = player.Id;
            }
            return View(topic);
        }


        [HttpPost]
        public IActionResult DeleteTopic(Topic topic)
        {
            int PokerRoomId;

            int PlayerId;

            int Password;

            using (var _context = new PokerPlanningContext())
            {
               
               topic = _context.Topics.Where(t => t.Id == topic.Id).SingleOrDefault();

                PokerRoomId = topic.PokerRoomId;

                Password = _context.PokerRooms.FirstOrDefault(rid => rid.Id == PokerRoomId).Password.Value;
                ViewBag.Password = Password;

                var player = _context.Players.Where(p => p.PokerRoomId == topic.PokerRoomId && p.Role == 2).SingleOrDefault();
                PlayerId = player.Id;

                var cards = _context.Cards.Where(c => c.TopicId == topic.Id).ToList();
                _context.Cards.RemoveRange(cards);

                _context.Topics.Remove(topic);
                _context.SaveChanges();
            }
            return RedirectToAction("RoomEntrance", new { PokerRoomId, PlayerId, Password });
        }
    }
}
