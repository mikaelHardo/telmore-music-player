using System;

namespace MusicPlayerNew
{
    public class Info
    {
        public string Cover { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Time { get; set; }

        public string Length { get; set; }

        public double Progress
        {
            get
            {
                var timeSeconds = ToSeconds(Time);
                var totalSeconds = ToSeconds(Length);

                if (totalSeconds == 0)
                {
                    return 0;

                }

                return timeSeconds / totalSeconds * 100;
            }
        }


        private double ToSeconds(string time)
        {
            var split = time.Split(':');
            return int.Parse(split[0]) * 60 + int.Parse(split[1]);
        }
    }
}
