// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Text;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    // lamedb version 5 ("eDVB services /5/") support.
    // v5 is a single-line re-serialization of the same transponders and services
    // the v3/v4 path uses. These helpers convert between the v4 block layout the
    // existing builders/parsers use and the v5 "t:" / "s:" line records.
    public partial class SettingsIO
    {
        protected const string LameDb5FileName = "lamedb5";

        /// <summary>
        ///     Converts the v4 transponder block layout
        ///     ("ns:tsid:nid" line, "\t&lt;type&gt; freq:..." line, "/" line, repeated)
        ///     into v5 transponder records ("t:ns:tsid:nid,&lt;type&gt;:freq:...").
        /// </summary>
        protected virtual string TranspondersToLameDb5(string v4TransponderBlocks)
        {
            var sb = new StringBuilder();
            string[] lines = (v4TransponderBlocks ?? string.Empty).Split('\n');
            for (int i = 0; i + 1 < lines.Length; i += 3)
            {
                string key = lines[i].Trim();
                if (key.Length == 0) { i -= 2; continue; } // skip stray blank line, advance by 1
                string content = lines[i + 1].TrimStart('\t', ' ');
                int sp = content.IndexOf(' ');
                string v5Content = sp > -1
                    ? content.Substring(0, sp) + ":" + content.Substring(sp + 1)
                    : content;
                sb.Append("t:").Append(key).Append(',').Append(v5Content).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Converts the v4 services section ("services" .. "end", each service a
        ///     data_id line / name line / flags line) into v5 service records
        ///     ("s:data_id,&quot;name&quot;[,flags]").
        /// </summary>
        protected virtual string ServicesToLameDb5(string v4ServicesSection)
        {
            var sb = new StringBuilder();
            string[] lines = (v4ServicesSection ?? string.Empty).Split('\n');
            int start = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == SettingsServicesOpenTag) { start = i + 1; break; }
            }
            if (start < 0) return string.Empty;
            for (int i = start; i + 2 < lines.Length; i += 3)
            {
                string dataId = lines[i].Trim();
                if (dataId.Length == 0 || dataId == SettingsClosingTag) break;
                string name = lines[i + 1];
                string flags = lines[i + 2];
                string flagsPart = (!string.IsNullOrEmpty(flags) && flags != "p:") ? "," + flags : string.Empty;
                sb.Append("s:").Append(dataId).Append(",\"").Append(name).Append('"').Append(flagsPart).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Splits a v5 transponder record "t:ns:tsid:nid,&lt;type&gt;:freq:..." into the
        ///     v4 first line ("ns:tsid:nid") and second line ("&lt;type&gt; freq:...").
        ///     Returns false if the line is malformed.
        /// </summary>
        protected virtual bool TryParseLameDb5Transponder(string line, out string firstLine, out string secondLine)
        {
            firstLine = null;
            secondLine = null;
            if (string.IsNullOrEmpty(line) || !line.StartsWith("t:")) return false;
            int comma = line.IndexOf(',');
            if (comma < 0) return false;
            firstLine = line.Substring(2, comma - 2).Trim();
            string content = line.Substring(comma + 1).Trim();
            int firstColon = content.IndexOf(':');
            secondLine = firstColon > -1
                ? content.Substring(0, firstColon) + " " + content.Substring(firstColon + 1)
                : content;
            return firstLine.Length > 0 && secondLine.Length > 0;
        }

        /// <summary>
        ///     Splits a v5 service record 's:data_id,"name"[,flags]' into its three v4 parts.
        ///     Returns false if the line is malformed.
        /// </summary>
        protected virtual bool TryParseLameDb5Service(string line, out string dataId, out string name, out string flags)
        {
            dataId = null;
            name = null;
            flags = "p:";
            if (string.IsNullOrEmpty(line) || !line.StartsWith("s:")) return false;
            string rest = line.Substring(2);
            int comma = rest.IndexOf(',');
            if (comma < 0) return false;
            dataId = rest.Substring(0, comma).Trim();
            string after = rest.Substring(comma + 1).Trim();
            if (after.StartsWith("\""))
            {
                int endQuote = after.IndexOf('"', 1);
                if (endQuote > 0)
                {
                    name = after.Substring(1, endQuote - 1);
                    string remainder = after.Length > endQuote + 1 ? after.Substring(endQuote + 1).Trim() : string.Empty;
                    if (remainder.StartsWith(",")) remainder = remainder.Substring(1).Trim();
                    if (remainder.Length > 0) flags = remainder;
                }
                else
                {
                    name = after.Trim('"');
                }
            }
            else
            {
                int c2 = after.IndexOf(',');
                name = c2 > -1 ? after.Substring(0, c2) : after;
                if (c2 > -1 && after.Length > c2 + 1) flags = after.Substring(c2 + 1).Trim();
            }
            return dataId.Length > 0;
        }
    }
}
