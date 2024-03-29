﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PlanningPoker
{
    public class PokerRoom
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TypeCards { get; set; }
        public int? Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Role { get; set; }
        public bool IsOnline { get; set; }

        public int PokerRoomId { get; set; }
        public PokerRoom PokerRoom { get; set; }
    }
    public class Message
    {
        public int Id { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }

        public int PokerRoomId { get; set; }
        public PokerRoom PokerRoom { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
    public class Topic
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не задано название задачи")]
        [Display(Name = "Задача")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Оценка")]
        public string Marks { get; set; }

        public int Status { get; set; }

        public int IterationNumb { get; set; }

        public int PokerRoomId { get; set; }
        public PokerRoom PokerRoom { get; set; }
    }
    public class Card
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string CardValue { get; set; }
        public int IterationNumb { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
