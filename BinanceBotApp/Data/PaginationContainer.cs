using System.Collections.Generic;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// Контейнер для поддержки постраничного просмотра таблиц
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationContainer<T>
    {
        public PaginationContainer()
        {
            Items = new List<T>(4);
        }

        public PaginationContainer(int capacity)
        {
            Items = new List<T>(capacity);
        }

        /// <summary>
        /// Кол-во записей пропущенных с начала таблицы в запросе от api
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Кол-во записей в запросе от api
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Кол-во записей всего в таблице
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Данные
        /// </summary>
        public List<T> Items { get; set; }
    }
}