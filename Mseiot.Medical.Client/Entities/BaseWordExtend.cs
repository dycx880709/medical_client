using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Client.Entities
{
    public class BaseWordExtend : BaseWord
    {
        public ObservableCollection<string> Items { get; set; }
        public BaseWordExtend(BaseWord word)
        {
            word.CopyTo(this);
            this.Items = new ObservableCollection<string>();
            if (!string.IsNullOrEmpty(word.Content))
                Items.AddRange(word.Content.Split(','));
            Items.CollectionChanged += (o, e) => this.Content = string.Join(",", this.Items);
        }

        public BaseWordExtend()
        {
            this.Items = new ObservableCollection<string>();
            Items.CollectionChanged += (o, e) => this.Content = string.Join(",", this.Items);
        }
    }
}
