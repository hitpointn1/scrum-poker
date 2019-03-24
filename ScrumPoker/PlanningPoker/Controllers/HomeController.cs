﻿using System;
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


                using (var context = new PokerPlanningContext())
                {
                    PokerRoom Room = context.PokerRooms.Where(m => m.Id == RoomId).SingleOrDefault<PokerRoom>();

                    if (string.Compare(Room.Password, Password) == 0)//Проверка пароля, регистр важен
                    {
                        Player NewPlayer;

                        NewPlayer = new Player()
                        {
                            Name = Name,
                            Role = 1,
                            PokerRoomId = RoomId
                        };

                        var Checker = context.Players
                            .Where(m => m.PokerRoomId == NewPlayer.PokerRoomId
                            && string.Compare(NewPlayer.Name, m.Name, true) == 0).ToList<Player>().Count;

                        if (Checker == 0)//Создать нового пользователя
                        {
                            context.Players.Add(NewPlayer);
                            context.SaveChanges();
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


        public IActionResult Info() //ScrumPoker rules
        {
            return View();
        }

        public IActionResult RoomsList()
        {
            using (var context = new PokerPlanningContext())
            {
                #region Работает, анонимные типы, не пишется в таблицу. Задать вопрос.
                //var RList = context.PokerRooms.ToList().OrderBy(m => m.CreateDate);

                //var Pcount = context.Players.ToList();

                //var result = RList.GroupJoin(Pcount, R => R.Id, P => P.PokerRoomId,
                //    (rl, pc) => new { rl.Id, rl.Title, PlayersCount = pc.Count() });

                //ViewBag.Result = result;

                //return View(ViewBag.Result);
                //Как парсить анонимные типы? в данном случае он передает в виде {id = {rl.Id},....}

                #endregion

                #region Вспомогательная модель, вроде тоже душно
                List<RoomListModel> roomListModels = new List<RoomListModel>();

                var RList = context.PokerRooms.ToList().OrderBy(m => m.CreateDate);

                foreach (var b in RList)
                {
                    var newmodel = new RoomListModel
                        (
                        b.Id,
                        b.Title, 
                        context.Players.Count<Player>(m => m.PokerRoomId == b.Id)
                        );
                    roomListModels.Add(newmodel);
                }

                return View(roomListModels);
                #endregion
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
