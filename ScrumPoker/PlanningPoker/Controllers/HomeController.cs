using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Models;

namespace PlanningPoker.Controllers
{
    public class HomeController : Controller
    {
        IHubContext<VotingHub> hubContext;
        public HomeController(IHubContext<VotingHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("Create")]
        public IActionResult RoomCreate(CreateModel Created)
        {
            // Creation of ScrumPoker room
            if (ModelState.IsValid)
            {
                int? _nullablepassword;
                using (var _context = new PokerPlanningContext())
                {
                    if (Created.Password is null)
                        _nullablepassword = null;
                    else
                        _nullablepassword = Created.Password.GetHashCode();

                    var NewRoom = new PokerRoom()
                    {
                        Title = Created.RoomTitle,
                        Description = "",
                        Password = _nullablepassword,
                        CreateDate = DateTime.Now,
                        CloseDate = null,
                        TypeCards = Created.CardsType
                    };
                    
                    _context.PokerRooms.Add(NewRoom);
                    _context.SaveChanges();

                    var NewPlayer = new Player()
                    {
                        Name = Created.Name,
                        Role = 2,
                        PokerRoomId = NewRoom.Id,
                        IsOnline = false
                    };

                    _context.Players.Add(NewPlayer);
                    _context.SaveChanges();

                    return RedirectToAction("RoomEntrance", new { PokerRoomId = NewRoom.Id, PlayerId = NewPlayer.Id }); //переход в комнату
                }
                
            }
            else
                return View(Created);
        }


        [HttpPost("Join")]
        public IActionResult RoomJoin(JoinModel Joined)
        {
            
            
            // Joining to ScrumPoker room
            if (ModelState.IsValid)
            {
                int? _nullablepassword;
                try
                {
                    using (var _context = new PokerPlanningContext())
                    {
                        if (Joined.Password is null)
                            _nullablepassword = null;
                        else
                            _nullablepassword = Joined.Password.GetHashCode();
                        var Room = _context.PokerRooms.Where(m => m.Id == Joined.RoomId).SingleOrDefault();

                        if (Equals(Room.Password, _nullablepassword))//Проверка пароля, регистр важен
                        {
                            var NewPlayer = new Player()
                            {
                                Name = Joined.Name,
                                Role = 1,
                                PokerRoomId = Joined.RoomId.Value,
                                IsOnline = false
                            };

                            var Checker = _context.Players
                                .Where(m => m.PokerRoomId == NewPlayer.PokerRoomId
                                && string.Compare(NewPlayer.Name, m.Name, true) == 0).ToList().Count;

                            if (Checker == 0)//Создать нового пользователя
                            {
                                _context.Players.Add(NewPlayer);
                                _context.SaveChanges();
                                return RedirectToAction("RoomDiscussion", new { PokerRoomId = Room.Id, PlayerId = NewPlayer.Id });//переход в комнату
                            }
                            else //Такой пользователь уже есть, зайти под ним
                            { 
                                var player = _context.Players.Where(p => p.PokerRoomId == Joined.RoomId && p.Name == NewPlayer.Name).SingleOrDefault();
                                if (player.Role == 2)
                                    return RedirectToAction("RoomEntrance", new { PokerRoomId = Room.Id, PlayerId = player.Id });//переход в комнату
                                else
                                    return RedirectToAction("RoomDiscussion", new { PokerRoomId = Room.Id, PlayerId = player.Id });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Неверный пароль");
                            return View(Joined);
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    ModelState.AddModelError("RoomId", "Неверный ID Комнаты");
                    return View(Joined);
                }
            }
            else
                return View(Joined);
        }

        [HttpGet("Info")]
        public IActionResult Info() //ScrumPoker rules
        {
            return View();
        }


        [HttpGet("List")]
        public IActionResult RoomsList()//Вывод списка комнат
        {
            using (var _context = new PokerPlanningContext())
            {
                var RList = _context.PokerRooms.ToList().OrderBy(m => m.CreateDate);
                var Pcount = _context.Players.ToList();

                var rooms = RList.GroupJoin(Pcount, R => R.Id, P => P.PokerRoomId, 
                    (rl, pc) => new RoomListModel(rl.Id, rl.Title, pc.Count()))
                    .ToList();

                return View(rooms);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                        PokerRoomId = PokerRoomId
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

                    if (Topic.Status == 1 &&  countTopicStart == 0)//если голосование необходимо запустить
                    {
                        Topic.Status = 2; // старт голосования
                    }
                    else if(Topic.Status == 2 && countTopicStart == 1)
                    {
                        Topic.Status = 1; // стоп голосования
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

                //Если тема обсуждается
                if (countTopicDiscussion != 0)
                {
                    ViewBag.CountTopicDiscussion = countTopicDiscussion;
                    ViewBag.Title = TopicDiscussion.SingleOrDefault().Title;
                    ViewBag.Description = TopicDiscussion.SingleOrDefault().Description;
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

    }
}
