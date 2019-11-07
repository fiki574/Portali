/*
    Live feed of Croatian public news portals
    Copyright (C) 2019 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.IO;

namespace Portals
{
    public static class Constants
    {
        public static readonly int
            HttpServerPort = 5465,
            ScrappersSleepInterval = 30 * 1000 * 60,
            MainThreadSleepInterval = 5000;

        public static readonly string 
            ImageData = "data:image/png;base64,@base64@", 
            Homepage = File.ReadAllText("html/index.html").Replace("@base64@", Utilities.GetBase64("html/images/portali.png"));
    }
}