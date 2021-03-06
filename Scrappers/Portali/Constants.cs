﻿/*
    Live feed of Croatian public news portals
    Copyright (C) 2020/2021 Bruno Fištrek

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
            ScrappersSleepInterval = 60 * 1000 * 60,
            MainThreadSleepInterval = 10000;

        public static readonly string
            Homepage = File.ReadAllText("html/index.html"),
            Redirect = "<!DOCTYPE html><html><head><meta http-equiv=\"Refresh\" content=\"0; url=@redurl@\"/></head><body></body></html>",
            Empty = "<!DOCTYPE html><html><head></head><body></body></html>";
    }
}