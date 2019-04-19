using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Models;
using PlanningPoker.Hubs;

namespace PlanningPoker.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet]
        public IActionResult Index(CreateModel Create, JoinModel Join)
        {
            return View((Create,Join));
        }


        [HttpPost("Create")]
        public IActionResult RoomCreate(CreateModel Created)
        {
            // Creation of ScrumPoker room
            if (ModelState.IsValid)
            {
                int _nullablepassword;
                using (var _context = new PokerPlanningContext())
                {

                    _nullablepassword = PasswordEncrypt.GetPassword(Created.Password);

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
                    return RedirectToAction("RoomEntrance", "ScrumRoom", new { PokerRoomId = NewRoom.Id, PlayerId = NewPlayer.Id, password = _nullablepassword }); //переход в комнату
                }

            }
            else
                return View(Created);
        }


        [HttpGet("Join")]
        public IActionResult RoomJoin(int Id)
        {
            var model = new JoinModel
            {
                RoomId = Id
            };
            return View(model);
        }

        [HttpPost("Join")]
        public IActionResult RoomJoin(JoinModel Joined)
        {
            // Joining to ScrumPoker room
            if (ModelState.IsValid)
            {
                int _nullablepassword;
                try
                {
                    using (var _context = new PokerPlanningContext())
                    {
                            _nullablepassword = PasswordEncrypt.GetPassword(Joined.Password);
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
                                return RedirectToAction("RoomDiscussion", "ScrumRoom", new { PokerRoomId = Room.Id, PlayerId = NewPlayer.Id, password = _nullablepassword });//переход в комнату
                            }
                            else //Такой пользователь уже есть, зайти под ним
                            {
                                
                                var player = _context.Players.Where(p => p.PokerRoomId == Joined.RoomId && p.Name == NewPlayer.Name).SingleOrDefault();
                                if (player.Role == 2)
                                    return RedirectToAction("RoomEntrance", "ScrumRoom", new { PokerRoomId = Room.Id, PlayerId = player.Id, password = _nullablepassword });//переход в комнату
                                else
                                    return RedirectToAction("RoomDiscussion", "ScrumRoom", new { PokerRoomId = Room.Id, PlayerId = player.Id, password = _nullablepassword });
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
        
        [NonAction]
        public IActionResult Chat(Message model)//Chat test
        {  
                return View("~/Views/Chat/_Chat.cshtml", model);
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

    }
}
