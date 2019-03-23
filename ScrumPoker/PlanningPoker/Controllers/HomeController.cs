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
            
            using (var context = new PokerPlanningContext())
            {
                Player NewPlayer; //Ведущий
                PokerRoom NewRoom; // Новая комната
                
                NewRoom = new PokerRoom()
                {
                    Title = RoomTitle,
                    Description = "",
                    Password = Password,
                    CreateDate = DateTime.Now,
                    CloseDate = DateTime.Now,
                    TypeCards = CardsType
                };
                context.Database.BeginTransaction();
                context.PokerRooms.Add(NewRoom);
                context.SaveChanges();
                NewPlayer = new Player()
                {
                    Name = Name,
                    Role = 2,
                    PokerRoomId = NewRoom.Id
                };
                context.Players.Add(NewPlayer);
                context.SaveChanges();
                context.Database.CommitTransaction();
            }
            return RedirectToAction("Index"); //Сделать: переход в комнату
        }


        [HttpPost]
        public IActionResult RoomJoin(string Name, int RoomId, string Password)
        {
            // Joining to ScrumPoker room
            using (var context = new PokerPlanningContext())
            {
                PokerRoom Room = context.PokerRooms.Where(m => m.Id == RoomId).SingleOrDefault<PokerRoom>();
                if (Room is null)
                {
                    return new BadRequestResult(); //Реализовать вывод в форму сообщения о неверности пароля, а так же проверка на правильность остальных введенных данных.
                }
                if (string.Compare(Room.Password, Password) == 0)
                {
                    Player NewPlayer;
                    NewPlayer = new Player()
                    {
                        Name = Name,
                        Role = 1,
                        PokerRoomId = RoomId
                    };
                    context.Players.Add(NewPlayer);
                    context.SaveChanges();
                    return RedirectToAction("Index");//Сделать: переход в комнату
                }
                else
                    return new BadRequestResult();
            }
        }


        public IActionResult Info() //ScrumPoker rules
        {
            return View();
        }

        public IActionResult RoomsList()//db.context.Rooms.ToList => View
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
