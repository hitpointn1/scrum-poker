using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Models;

namespace PlanningPoker.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult RoomCreate(CreateModel Created)
        {
            // Creation of ScrumPoker room
                if (ModelState.IsValid)
                {
                    using (var _context = new PokerPlanningContext())
                    {
                        var NewRoom = new PokerRoom()
                        {
                            Title = Created.RoomTitle,
                            Description = "",
                            Password = Created.Password,
                            CreateDate = DateTime.Now,
                            CloseDate = null,
                            TypeCards = Created.CardsType
                        };

                        _context.Database.BeginTransaction();

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

                        _context.Database.CommitTransaction();
                    }
                    return RedirectToAction("Index"); //Сделать: переход в комнату
                }
                else
                    return View(Created);
        }


        [HttpPost]
        public IActionResult RoomJoin(JoinModel Joined)
        {
            // Joining to ScrumPoker room
            if (ModelState.IsValid)
            {
                try
                {
                    using (var _context = new PokerPlanningContext())
                    {
                        var Room = _context.PokerRooms.Where(m => m.Id == Joined.RoomId).SingleOrDefault();

                        if (string.Compare(Room.Password, Joined.Password) == 0)//Проверка пароля, регистр важен
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
                                return RedirectToAction("Index");//Сделать: переход в комнату
                            }
                            else //Такой пользователь уже есть, зайти под ним
                                return RedirectToAction("Index");//Сделать: переход в комнату
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
    }
}
