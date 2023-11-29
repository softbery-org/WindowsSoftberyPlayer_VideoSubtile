using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoSubtile
{
    /// <summary>
    /// Example srt file
    /// 1
    /// 00:00:03,400 --> 00:00:06,177
    /// In this lesson, we're going to
    /// be talking about finance. And
    /// 
    /// 2
    /// 00:00:06,177 --> 00:00:10,009
    /// one of the most important aspects
    /// of finance is interest.
    /// 
    /// 3
    /// 00:00:10,009 --> 00:00:13,655
    /// When I go to a bank or some
    /// other lending institution
    /// 
    /// 4
    /// 00:00:13,655 --> 00:00:17,720
    /// to borrow money, the bank is happy
    /// to give me that money. But then I'm
    /// 
    /// 5
    /// 00:00:17,900 --> 00:00:21,480
    /// going to be paying the bank for the
    /// privilege of using their money. And that
    /// 
    /// 6
    /// 00:00:21,660 --> 00:00:26,440
    /// amount of money that I pay the bank is
    /// called interest. Likewise, if I put money
    /// </summary>
    public static class fromString
    {
        public static TimeSpan GetTimeSpan(this string txt)
        {
            txt = txt.Trim();
            return TimeSpan.Parse(txt);
        }
    }

    public class SubtileManager
    {
        private static string _subtilesFile;
        private List<Subtile> _startTimeList = new List<Subtile>();
        private Dictionary<string, Subtile> _subtiles = new Dictionary<string, Subtile>();
        public int Count { get => _subtiles.Count; }
        public Subtile[] StartList { get => _startTimeList.ToArray(); }
        public Subtile this[string start_time]
        {
            get => _subtiles[$"{start_time}"];
            set => _subtiles[$"{start_time}"] = value;
        }

        public SubtileManager()
        {

        }

        public SubtileManager(string path)
        {
            _subtilesFile = path;
            readSRT(path);
        }

        public TimeSpan[] StartTimeList()
        {
            var startTimeList = new List<TimeSpan>();
            foreach (var item in _startTimeList)
            {
                startTimeList.Add(fromString.GetTimeSpan(item.StartTime));
            }
            return startTimeList.ToArray();
        }

        public TimeSpan[] StartTimeList(string pattern)
        {
            var startTimeList = new List<TimeSpan>();
            foreach (var item in _subtiles)
            {
                var st = fromString.GetTimeSpan(item.Key);
                var result = $"{st.Hours:00}:{st.Minutes:00}:{st.Seconds:00}";
                this[item.Key] = item.Value;
                startTimeList.Add(fromString.GetTimeSpan(result));
            }
            return startTimeList.ToArray();
        }

        public string getS(ref string txt)
        {
            return txt;
        }

        private void readSRT(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                var read = File.ReadAllLines(file.FullName);

                for (int j = 0; j < read.Length - 1; j++)
                {
                    var regexExpTime = $"(\\d+).*(\\d+:\\d+:\\d+,\\d+).+-->.+(\\d+:\\d+:\\d+,\\d+)";

                    var regex = new Regex(regexExpTime);

                    var match = regex.Matches(read[j]);
                    var s = new Subtile();

                    if (match.Count > 0)
                    {
                        s.Id = Convert.ToInt32(read[j - 1]);
                        var split = match[0].Groups[0].Value.Replace("-->", "|");
                        var splits = split.Split('|');
                        s.StartTime = splits[0];
                        var t = TimeSpan.Parse(s.StartTime);
                        var StartTimeKey = $"{t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}";
                        s.EndTime = splits[1];
                        t = TimeSpan.Parse(s.EndTime);
                        s.EndTime = $"{t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}";
                        s.Text1 = read[j + 1];
                        s.Text2 = read[j + 2];
                        var id = read[j - 1];
                        var start = match[0].Groups[0].Value;
                        var end = match[0].Groups[1].Value;

                        _subtiles.Add(StartTimeKey, s);
                    }
                }
            }

        }


        public bool isExist(string txt)
        {
            return _subtiles.ContainsKey(txt);
        }

        private TimeSpan timerSpan(string time)
        {
            return TimeSpan.Parse(time);
        }

        public Subtile GetSubtile(string start_time)
        {
            foreach (var subtile in _subtiles)
            {
                if (subtile.Key == start_time)
                {
                    var s = new Subtile();
                    return subtile.Value;
                }
            }
            return null;
        }

        public Dictionary<string, Subtile> GetSubStartTime()
        {
            var d = new Dictionary<string, Subtile>();
            foreach (var sub in _subtiles.Values)
            {
                d.Add(sub.StartTime, sub);
            }
            return d;
        }

        public Dictionary<string, Subtile> GetSubtiles()
        {
            return _subtiles;
        }
        /*public class SubtileManager
        {
            private Dictionary<TimeSpan, Subtile> _subtiles = new Dictionary<TimeSpan, Subtile>();
            private List<SubtileSrt> _subtilesList = new List<SubtileSrt>();

            public Dictionary<TimeSpan, Subtile> SubtilesDictionary { get => _subtiles; set=>_subtiles = value; }
            public List<SubtileSrt> Subtiles { get => _subtilesList; set => _subtilesList = value; }

            public SubtileManager() { }

            public TimeSpan GetTimeFromString(string time)
            {
                return TimeSpan.Parse(time);
            }

            public TimeSpan GetTimeFromDouble(double val)
            {
                return TimeSpan.FromSeconds(val);
            }

            public void AddLinesContent(string start_time, string end_time, string[] line_content)
            {
                if (!_subtiles.ContainsKey(GetTimeFromString(start_time)))
                {
                    var s = new Subtile();
                    s.VisibleDuration = GetTimeFromString(start_time) - GetTimeFromString(end_time);
                    s.LineContent = new string[line_content.Length];
                    int i = 0;
                    foreach (var line in line_content)
                    {
                        s.LineContent[i] = line;
                        i++;
                    }
                    _subtiles.Add(GetTimeFromString(start_time), s);
                }
            }

            public void RemoveLinesContent(string time)
            {
                _subtiles.Remove(GetTimeFromString(time));
            }

            public Subtile GetLinesContent(string time)
            {
                if (_subtiles.ContainsKey(GetTimeFromString(time)))
                {
                    return _subtiles[GetTimeFromString(time)];
                }else
                    return null;
            }


            public void ReadFromFile(string path)
            {
                var file = new FileInfo(path);
                if (file.Exists)
                {
                    readSRT(path);
                }
            }

            public void WriteToFile(string path)
            {

            }

            private void readSRT(string path)
            {
                var fileContent = File.ReadAllLines(path);
                if (fileContent.Length <= 0)
                    return;

                _subtilesList = new List<SubtileSrt>();
                SubtilesDictionary = new Dictionary<TimeSpan, Subtile>();
                var segment = 1;
                for (int item = 0; item < fileContent.Length; item++)
                {
                    if (segment.ToString() == fileContent[item])
                    {
                        var st = fileContent[item + 1].Substring(0, fileContent[item + 1].LastIndexOf("-->")).Trim();
                        var s = new Subtile();
                        s.VisibleDuration = GetTimeFromString(fileContent[item + 1].Substring(fileContent[item + 1].LastIndexOf("-->") + 3).Trim());
                        s.LineContent = new string[] { fileContent[item + 2], fileContent[item + 3] };
                        SubtilesDictionary.Add(GetTimeFromString(st), s);
                        _subtilesList.Add(new SubtileSrt
                        {
                            Segment = segment.ToString(),
                            StartTime = fileContent[item + 1].Substring(0, fileContent[item + 1].LastIndexOf("-->")).Trim(),
                            EndTime = fileContent[item + 1].Substring(fileContent[item + 1].LastIndexOf("-->") + 3).Trim(),
                            TextI = fileContent[item + 2],
                            TextII = fileContent[item + 3]
                        });
                        // The block numbers of SRT like 1, 2, 3, ... and so on
                        segment++;
                        // Iterate one block at a time
                        item += 3;
                    }
                }             
            }

            public Dictionary<string, Subtile> GetSubtiles()
            {
                var result = new Dictionary<string, Subtile>();
                foreach (var sub in _subtiles)
                {
                    result.Add(sub.Key.ToString(), sub.Value);
                }
                return result;
            }*/
    }
}
