using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlavysMod.Models
{
    public class HtmlLibrary
    {
        private static readonly string bodyStyle = "style=\"font-family: Arial, sans-serif; background-color: #202124; margin: 0; padding: 20px;\"";
        private static readonly string headerStyle = "style=\"color: #3ab3ff; font-size: 36px;\"";
        private static readonly string paragraphStyle = "style=\"font-size: 18px; color: #f4f4f4;\"";
        private static readonly string linkStyle = "style=\"color: #2196F3; text-decoration: none; font-weight: bold;\"";
        private static readonly string patreonLink = "\"https://patreon.com/Slavy\"";

        private readonly string index = $"<body {bodyStyle}>\r\n" +
                        $"    <h1 {headerStyle}>Server Is Running! Sweet!!</h1>\r\n" +
                        $"    <p {paragraphStyle}>Try clicking <a href=\"spawn_meleeattacker\" {linkStyle}>here</a> to spawn some attackers</p>\r\n" +
                        $"    <p {paragraphStyle}><a href=\"logs\" {linkStyle}>View Logs</a> to see what's going on</p>\r\n" +
                        $"    <p {paragraphStyle}>For additional information and support, please visit the <a href={patreonLink} {linkStyle}>Slavy's Patreon</p>\r\n" +
                        $"    <p {paragraphStyle}Thank you for using this mod! I hope you find it as enjoyable to use as it was to create!</p>\r\n" +
                        "</body>";

        public string GetIndexPage()
        {
            return index;
        }
    }
}
