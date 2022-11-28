using Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Services
{
    internal class DelieveryHandling
    {
        Action<Order> RemoveOrderFromDelievery;
        private readonly int buisnessDelieveryTime;

        public DelieveryHandling(Action<Order> RemoveOrderFromDelievery , int buisnessDelieveryTime)
        {
            this.RemoveOrderFromDelievery = RemoveOrderFromDelievery;
            this.buisnessDelieveryTime = buisnessDelieveryTime;
        }


        public async Task DelieveryHandler(Order order)
        {
            await Task.Delay(buisnessDelieveryTime * 1000);
            RemoveOrderFromDelievery.Invoke(order);
        }
    }
}
