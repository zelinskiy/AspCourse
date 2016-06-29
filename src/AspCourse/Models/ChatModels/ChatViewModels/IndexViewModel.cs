using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels.ChatViewModels
{
    public class IndexViewModel
    {

        public List<Tuple<Topic, List<Message>, DateTime>> Previews { get; set; }

        public bool IsModer { get; set; }

    }
    

}
