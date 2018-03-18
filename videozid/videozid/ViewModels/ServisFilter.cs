using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    public class ServisFilter
    {

        public int? IdStart { get; set; }
        public int? IdStop { get; set; }

        public bool IsEmpty()
        {
            bool active = IdStart.HasValue || IdStop.HasValue;
            return !active;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}",
                IdStart, IdStop);
        }

        public static ServisFilter FromString(string s)
        {
            var filter = new ServisFilter();
            var arr = s.Split(new char[] { '-' }, StringSplitOptions.None);
            try
            {
                filter.IdStart = string.IsNullOrWhiteSpace(arr[0]) ? new Int32() : Int32.Parse(arr[0]);
                filter.IdStop = string.IsNullOrWhiteSpace(arr[1]) ? new Int32() : Int32.Parse(arr[1]);
            }
            catch { } //to do: log...
            return filter;
        }

        public IQueryable<Servis> Apply(IQueryable<Servis> query)
        {
            
            if (IdStart > 0)
            {
                query = query.Where(d => d.Id >= IdStart);
            }
            if (IdStop > 0)
            {
                query = query.Where(d => d.Id <= IdStop);
            }

            return query;
        }
    }
}
