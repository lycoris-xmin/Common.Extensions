namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlCheckHelper
    {
        //Sql注入时,可能出现的sql关键字,可根据自己的实际情况进行初始化,每个关键字由'|'分隔开来
        //private const string StrKeyWord=@"select|insert|delete|from|count(|drop table|update|truncate|asc(|mid(|char(|xp_cmdshell|exec master|netlocalgroup administrators|:|net user|""|or|and";
        private const string StrKeyWord = @"select|insert|delete|from|drop table|update|truncate|exec master|netlocalgroup administrators|:|net user|or|and";
        //Sql注入时,可能出现的特殊符号,,可根据自己的实际情况进行初始化,每个符号由'|'分隔开来
        //private const string StrRegex=@"-|;|,|/|(|)|[|]|}|{|%|@|*|!|'";
        private const string StrRegex = @"=|!|'";

        /// <summary>
        /// 检查_sword是否包涵SQL关键字
        /// </summary>
        /// <param name="word">需要检查的字符串</param>
        /// <returns>存在SQL注入关键字时返回 true，否则返回 false</returns>
        public static bool CheckKeyWord(string word)
        {
            var result = false;
            //模式1 : 对应Sql注入的可能关键字
            string[] patten1 = StrKeyWord.Split('|');

            //模式2 : 对应Sql注入的可能特殊符号
            string[] patten2 = StrRegex.Split('|');

            //开始检查 模式1:Sql注入的可能关键字 的注入情况
            foreach (string sqlKey in patten1)
            {
                if (word.ToLower().Contains(" " + sqlKey, StringComparison.CurrentCulture) || word.ToLower().Contains(sqlKey + " ", StringComparison.CurrentCulture))
                {
                    //只要存在一个可能出现Sql注入的参数,则直接退出
                    result = true;
                    break;
                }
            }

            //开始检查 模式1:Sql注入的可能特殊符号 的注入情况
            foreach (string sqlKey in patten2)
            {
                if (word.ToLower().Contains(sqlKey, StringComparison.CurrentCulture))
                {
                    //只要存在一个可能出现Sql注入的参数,则直接退出
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
