using System;
using System.Collections.Generic;
using System.Linq;

namespace finalproject
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            List<string> variebles = new List<string>();
            List<string> terminals = new List<string>();

            Dictionary<string, List<string>> trans = new Dictionary<string, List<string>>();

            for (int i = 0; i < n; i++)
            {
                string[] completeTrans = Console.ReadLine().Split(new char[] {' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
                variebles.Add(completeTrans[0]);

                trans.Add(completeTrans[0].ToString(), new List<string>());

                for (int j = 2; j < completeTrans.Length; j++)
                {
                    string s = "";

                    if (completeTrans[j] == "#")
                    {
                        trans[completeTrans[0]].Add("");
                        continue;
                    }

                    for (int k = 0; k < completeTrans[j].Length; k++)
                    {
                        if (completeTrans[j][k] != '>' && completeTrans[j][k] != '<' && completeTrans[j][k].ToString() != "")
                        {
                            s += completeTrans[j][k];

                            if (k>0 && completeTrans[j][k - 1] != '<' && !terminals.Contains(completeTrans[j][k].ToString()))
                                terminals.Add(completeTrans[j][k].ToString());
                        }
                    }
                    trans[completeTrans[0]].Add(s);
                }
            }

            Dictionary<string, bool> nullid = new Dictionary<string, bool>();

            for (int i = 0; i < variebles.Count; i++)
            {
                nullid.Add(variebles[i], false);
            }

            RemoveNullble(trans, nullid);
            List<Tuple<string, string>> unitProductDeleted = new List<Tuple<string, string>>();
            RemoveUnit(trans, ref unitProductDeleted, variebles);

            foreach (KeyValuePair<string, List<string>> item in trans)
            {
                string s = item.Key + " -> ";

                for (int i = 0; i < item.Value.Count - 1; i++)
                {
                    s += item.Value[i] + " | ";
                }

                s += item.Value[item.Value.Count - 1];

                Console.WriteLine(s);
            }

            Console.WriteLine("-----------------------------------------------------");

            FirstStepToConvertToCNF(trans, variebles,terminals, n);

            foreach (KeyValuePair<string, List<string>> item in trans)
            {
                string s = item.Key + " -> ";

                for (int i = 0; i < item.Value.Count - 1; i++)
                {
                    s += item.Value[i] + " | ";
                }

                s += item.Value[item.Value.Count - 1];

                Console.WriteLine(s);
            }

            Console.WriteLine("-----------------------------------------------------");

            SecondStepToConvertToCNF(trans, variebles,terminals);

            foreach (KeyValuePair<string, List<string>> item in trans)
            {
                string s = item.Key + " -> ";

                for (int i = 0; i < item.Value.Count - 1; i++)
                {
                    s += item.Value[i] + " | ";
                }

                s += item.Value[item.Value.Count - 1];

                Console.WriteLine(s);
            }
        }

        private static void SecondStepToConvertToCNF(Dictionary<string, List<string>> trans, List<string> variebles, List<string> terminals)
        {
            int n = trans.Count;

            for (int i = 0; i < trans.Count; i++)
            {
                KeyValuePair<string, List<string>> tr = trans.ElementAt(i);

                List<string> lt = tr.Value;

                for (int j = 0; j < lt.Count; j++)
                {
                    if(lt[j].Length > 2)
                    {
                        if (n + 64 == 'S')
                            n++;
                        char newVar = (char)(n + 64);
                        
                        variebles.Add(newVar.ToString());
                        n++;

                        trans.Add(newVar.ToString(), new List<string> { lt[j].Substring(0,2)});

                        lt[j] = newVar.ToString()+lt[j].Substring(2);
                        j--;
                    }
                }
            }
        }
        private static void FirstStepToConvertToCNF(Dictionary<string, List<string>> trans, List<string> variebles, List<string> terminals, int n)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();

            for (int i = 0; i < terminals.Count; i++)
            {
                temp.Add(terminals[i], ((char)(n + 64 + i)).ToString());
            }

            for (int i = 0; i < trans.Count; i++)
            {
                KeyValuePair<string, List<string>> tr = trans.ElementAt(i);

                List<string> lt = tr.Value;

                for (int j = 0; j < lt.Count; j++)
                {
                    if (lt[j].Length == 1)
                        continue;
                    for (int k = 0; k < lt[j].Length; k++)
                    {
                        if (terminals.Contains(lt[j][k].ToString()))
                        {
                            lt[j] = lt[j].Substring(0, k) + temp[lt[j][k].ToString()] + lt[j].Substring(k + 1);
                        }
                    }
                }
            }

            for (int i = 0; i < temp.Count; i++)
            {
                KeyValuePair<string, string> t = temp.ElementAt(i);

                trans.Add(t.Value, new List<string> { t.Key});

                variebles.Add(t.Value);
            }
            //for (int i = 0; i < terminals.Count; i++)
            //{
            //    trans.Add(((char)(n + 64 + i)).ToString(), new List<string> { terminals[i] });
            //    variebles.Add(((char)(n + 64 + i)).ToString());
            //}
        }

        private static void RemoveUnit(Dictionary<string, List<string>> trans, ref List<Tuple<string, string>> unitProductDeleted, List<string> variebles)
        {
            bool flag = false;
            string varie = "";
            string dest = "";

            foreach (KeyValuePair<string, List<string>> tr in trans)
            {
                foreach (var des in tr.Value)
                {
                    if (variebles.Contains(des))
                    {
                        flag = true;
                        varie = tr.Key;
                        dest = des;

                        break;
                    }

                }

                if (flag)
                    break;
            }

            if (flag)
            {
                RemoveUnitProduct(trans, ref unitProductDeleted, varie, dest, variebles);
                unitProductDeleted.Add(Tuple.Create(varie, dest));
                RemoveUnit(trans, ref unitProductDeleted, variebles);
            }
        }

        private static void RemoveUnitProduct(Dictionary<string, List<string>> trans, ref List<Tuple<string, string>> unitProductDeleted, string varie, string des, List<string> variebles)
        {
            trans[varie].Remove(des);

            if (!unitProductDeleted.Contains(Tuple.Create(varie, des)))
                if (varie != des)
                    for (int i = 0; i < trans[des].Count; i++)
                    {
                        if (!trans[varie].Contains(trans[des][i]))
                            trans[varie].Add(trans[des][i]);
                    }
        }

        private static void RemoveNullble(Dictionary<string, List<string>> trans, Dictionary<string, bool> nullid)
        {
            bool flag = false;
            string varie = "";
            foreach (KeyValuePair<string, List<string>> tr in trans)
            {
                foreach (var des in tr.Value)
                {
                    if (des == "")
                    {
                        flag = true;
                        varie = tr.Key;
                        break;
                    }
                }

                if (flag)
                    break;
            }

            if (flag)
            {
                RemoveNullbleProduct(trans, varie, nullid);
                nullid[varie] = true;
                RemoveNullble(trans, nullid);
            }

        }

        private static void RemoveNullbleProduct(Dictionary<string, List<string>> trans, string varie, Dictionary<string, bool> nullid)
        {
            trans[varie].Remove("");
            if (!nullid[varie])
                for (int i = 0; i < trans.Count; i++)
                {
                    KeyValuePair<string, List<string>> tr = trans.ElementAt(i);
                    int trValCount = tr.Value.Count;

                    for (int j = 0; j < tr.Value.Count; j++)
                    {
                        string des = tr.Value[j];

                        if (des.Contains(varie))
                            for (int k = 0; k < des.Length; k++)
                            {
                                if (des[k] == varie[0])
                                    if (!tr.Value.Contains(des.Substring(0, k) + des.Substring(k + 1)))
                                        tr.Value.Add(des.Substring(0, k) + des.Substring(k + 1));
                            }

                    }
                    if (trValCount != tr.Value.Count)
                        i--;
                }
        }

    }
}
