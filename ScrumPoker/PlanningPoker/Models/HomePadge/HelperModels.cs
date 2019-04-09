using System.ComponentModel.DataAnnotations;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PlanningPoker.Models
{
    public class RoomListModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        
        public RoomListModel (int Id, string Title, int Count)
        {
            this.Id = Id;
            this.Title = Title;
            this.Count = Count;
        }
    }
    public static class PasswordEncrypt
    {

        public static int GetPassword(string _Password)
        {
            if (string.IsNullOrEmpty(_Password)) return 0;
            int hash = 17;
            MD5 md5 = MD5.Create();

            hash *= _Password.Length * 322;
            var mdhash = md5.ComputeHash(Encoding.UTF8.GetBytes(_Password));
            foreach (var item in mdhash)
                hash *= item;
            return hash;
        }
    }

  
    public class CreateModel
    {
        [Required(ErrorMessage = "Не задано имя ведущего")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не задано название комнаты")]
        [Display(Name = "Название комнаты")]
        public string RoomTitle { get; set; }

        [Display(Name = "Тип карт")]
        public int CardsType { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

    }
    public class JoinModel
    { 
        [Required(ErrorMessage = "Не задано имя игрока")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле ID комнаты не заполнено")]
        [Range(0,int.MaxValue, ErrorMessage ="Неверный ID комнаты")]
        [Display(Name = "ID комнаты")]
        public int? RoomId { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

    }

}
