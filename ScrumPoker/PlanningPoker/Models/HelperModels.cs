using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
}
