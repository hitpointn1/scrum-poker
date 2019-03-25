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
        public IActionResult RoomCreate(string Name, string RoomTitle, int CardsType, string Password)
        {
            // Creation of ScrumPoker room
            try
            {
                if (Name is null)
                    throw new ArgumentNullException("Name");
                if (RoomTitle is null)
                    throw new ArgumentNullException("RoomTitle");
                if (Password is null)
                    throw new ArgumentNullException("Password");

                using (var _context = new PokerPlanningContext())
                {
                    var NewRoom = new PokerRoom()
                    {
                        Title = RoomTitle,
                        Description = "",
                        Password = Password,
                        CreateDate = DateTime.Now,
                        CloseDate = null,
                        TypeCards = CardsType
                    };

                    _context.Database.BeginTransaction();

                    _context.PokerRooms.Add(NewRoom);

                    _context.SaveChanges();

                    var NewPlayer = new Player()
                    {
                        Name = Name,
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
            catch (ArgumentNullException)
            {
                return new BadRequestResult();//Сделать: Отправка в форму, что не введено;
            }
        }


        [HttpPost]
        public IActionResult RoomJoin(string Name, int RoomId, string Password)
        {
            // Joining to ScrumPoker room
            try
            {
                if (Name is null)
                    throw new ArgumentNullException("Name");
                if (RoomId < 1)
                    throw new ArgumentException("Неверный айди комнаты","RoomId");
                if (Password is null)
                    throw new ArgumentNullException("Password");


                using (var _context = new PokerPlanningContext())
                {
                    var Room = _context.PokerRooms.Where(m => m.Id == RoomId).SingleOrDefault();

                    if (string.Compare(Room.Password, Password) == 0)//Проверка пароля, регистр важен
                    {
                        var NewPlayer = new Player()
                        {
                            Name = Name,
                            Role = 1,
                            PokerRoomId = RoomId,
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
                    else//Неверный пароль
                        return new BadRequestResult(); //Реализовать вывод в форму сообщения о неверности пароля.
                }
            }
            catch (NullReferenceException)//Такой комнаты нет
            {
                return new BadRequestResult();
            }
            catch (ArgumentNullException)//Не заполнено поле Сделать: Отправка в форму, что не введено;
            {
                return new BadRequestResult();
            }
            catch (ArgumentException)//Неверный RoomId(Меньше 1)
            {
                return new BadRequestResult();
            }
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
