﻿using Ms.Libs.Models;
using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MM.Medical.Client.Core
{
    internal static class Extension
    {
        public static List<string> SplitContent(this MsResult<List<BaseWord>> result, string title, char separator = ',')
        {
            if (result.Content == null) return null;
            var word = result.Content.FirstOrDefault(t => t.Title.Equals(title));
            if (word == null) return null;
            else return string.IsNullOrEmpty(word.Content) ? null : word.Content.Split(separator).ToList();
        }
    }
}
