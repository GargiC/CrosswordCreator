using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sw3
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        List<location> l1 = new List<location>();
        //A default string for demo
        string[] abb = new string[] { "earth", "planet", "space", "mars", "jupiter", "mercury", "galaxy" };
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates the most nested embedded Crossword puzzle from a string of words
        /// </summary>
        private void CreateCrossword()
        {
            //STEP1: Initialize a datatable with 100 rows X 100 columns
            for (int j = 0; j < 100; j++)
            {
                DataColumn dc = new DataColumn(j.ToString(), System.Type.GetType("System.String"));
                dt.Columns.Add(dc);
            }
            for (int i = 0; i < 100; i++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
            }

            //STEP2: generate a unique pattern of 2 word combination from a string array of words
            //Example output: 
            //a1.Add("earth,planet");
            //a1.Add("planet,space");
            //a1.Add("space,mars");
            //a1.Add("mars,jupiter");
            //a1.Add("jupiter,mercury");
            //a1.Add("mercury,galaxy");
            List<string> a1 = new List<string>();
            string t1 = "";
            for (int u = 0; u < abb.Count(); u++)
            {
                if (t1 == "")
                {
                    t1 = abb[u];
                }
                else
                {
                    t1 = t1 + "," + abb[u];
                    a1.Add(t1);
                    t1 = abb[u];
                }
            }
            //STEP3: Initial beginning coordinates in the grid
            int x = 2;
            int y = 10;
            foreach (string a in a1)
            {
                //STEP4: Check to see if a T is possible or not
                theT _T = DetectT(a.Split(',')[0], a.Split(',')[1]);
                if (_T != null)
                {
                    //STEP4.1: We know a T is detected. Check which word already exists in the grid
                    location d1 = findWord(a.Split(',')[0]); //vert word
                    location d2 = findWord(a.Split(',')[1]); //horz word
                    if (d1 != null && d2 != null)
                    {
                        //case 1:
                        //If both the words are already in the grid dont do anything
                        //A false combination which is rare to happen
                        //TODO: Log it for weird cases, just in case
                    }
                    else
                    {
                        if (d1 == null && d2 == null)
                        {
                            //None of the words are present in the existing grid
                            //So neither of the word can be joined yet
                            if (l1.Count > 0)
                            {
                                //Few words already exist on the grid
                                int maxDepth = 0;
                                //Calculate the max depth of the puzzle, so far
                                for (int yy = 0; yy < l1.Count; yy++)
                                {
                                    if (maxDepth < l1[yy].y)
                                    {
                                        maxDepth = l1[yy].y;
                                    }
                                }
                                //Place the new pair, after a blank row
                                placeT(_T, x, maxDepth + _T.vertW.Length + 1);
                            }
                            else
                            {
                                //The first 2 words coming on an empty grid
                                placeT(_T, x, y);
                            }
                        }
                        else if (d2 == null && d1 != null)
                        {
                            //One word already exists in the grid and another is new
                            if (d1.hAlign == false) //the existing word is vertical
                            {
                                //Check if its possible to place the new word at all
                                //Check if all the necessary spots are available
                                bool couldPlaceIt = placeWordHoriz2(a.Split(',')[1], d1.x - _T.horizIndex, d1.y + _T.vertIndex);
                                if (couldPlaceIt == false) //Sorry can not place it with its pair
                                {
                                    //try to place it with all other existing words in the grid
                                    for (int u1 = 0; u1 < (l1.Count - 1); u1++)
                                    {
                                        theT _newT = new theT();
                                        if (l1[u1].hAlign == true)
                                        {
                                            //existing word is already horiz alligned
                                            _newT = DetectT(a.Split(',')[1], l1[u1].word);
                                        }
                                        else
                                        {
                                            //existing word on the grid is vert alligned
                                            _newT = DetectT(l1[u1].word, a.Split(',')[1]);
                                        }
                                        if (_newT != null)
                                        {
                                            location d11 = l1[u1];
                                            if (d11.hAlign == true) //existing word is horizontal
                                            {
                                                bool couldPlaceIt2 = placeWordVert2(a.Split(',')[1], l1[u1].x + _newT.horizIndex, l1[u1].y - _newT.vertIndex);
                                                if (couldPlaceIt2) break;
                                            }
                                            else
                                            {
                                                bool couldPlaceIt2 = placeWordHoriz2(a.Split(',')[1], l1[u1].x - _newT.horizIndex, l1[u1].y + _newT.vertIndex);
                                                if (couldPlaceIt2) break;
                                            }
                                        }
                                    }
                                }
                            }
                            else //existing word is horizontal
                            {
                                //Repeat the same steps as done for vertical in the if-loop
                                bool couldPlaceIt = placeWordVert2(a.Split(',')[1], d1.x + _T.horizIndex, d1.y - _T.vertIndex);
                                if (couldPlaceIt == false)
                                {
                                    //try to place it with other existing words in the grid
                                    for (int u1 = 0; u1 < (l1.Count - 1); u1++)
                                    {
                                        theT _newT = new theT();
                                        if (l1[u1].hAlign == true)
                                        {
                                            //existing word is already horiz alligned
                                            _newT = DetectT(a.Split(',')[1], l1[u1].word);
                                        }
                                        else
                                        {
                                            //existing word on the grid is vert alligned
                                            _newT = DetectT(l1[u1].word, a.Split(',')[1]);
                                        }
                                        if (_newT != null)
                                        {
                                            location d11 = l1[u1];
                                            if (d11.hAlign == true) //existing word is horizontal
                                            {
                                                bool couldPlaceIt2 = placeWordVert2(a.Split(',')[1], l1[u1].x + _newT.horizIndex, l1[u1].y - _newT.vertIndex);
                                                if (couldPlaceIt2) break;
                                            }
                                            else
                                            {
                                                bool couldPlaceIt2 = placeWordHoriz2(a.Split(',')[1], l1[u1].x - _newT.horizIndex, l1[u1].y + _newT.vertIndex);
                                                if (couldPlaceIt2) break;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                else  //No T detected
                {
                    //STEP4.2: see if the incoming horizontal word forms a T with any other already existing words in the grid
                    string incomingHWord = a.Split(',')[1];
                    //try to place it with other existing words in the grid
                    for (int u1 = 0; u1 < (l1.Count - 1); u1++)
                    {
                        theT _newT = new theT();
                        if (l1[u1].hAlign == true)
                        {
                            //existing word is already horiz alligned
                            _newT = DetectT(incomingHWord, l1[u1].word);
                        }
                        else
                        {
                            //existing word on the grid is vert alligned
                            _newT = DetectT(l1[u1].word, incomingHWord);
                        }

                        if (_newT != null)
                        {
                            location d11 = l1[u1];
                            if (d11.hAlign == true) //existing word is horizontal
                            {
                                bool couldPlaceIt2 = placeWordVert2(a.Split(',')[1], l1[u1].x + _newT.horizIndex, l1[u1].y - _newT.vertIndex);
                                if (couldPlaceIt2) break;
                            }
                            else
                            {
                                bool couldPlaceIt2 = placeWordHoriz2(a.Split(',')[1], l1[u1].x - _newT.horizIndex, l1[u1].y + _newT.vertIndex);
                                if (couldPlaceIt2) break;
                            }
                        }
                    }
                }
            }
            //Delete any empty 2 or more consecutive rows to improve readability
            DeleteEmptyRows();
            dataGridView1.AutoSize = true;
            dataGridView1.DataSource = dt;
        }

        /// <summary>
        /// This function will delete all empty rows except one to improve the readability of the Crossword
        /// After the Crossword puzzle is ready, this function needs to be run to get rid of unnecessary rows
        /// </summary>
        private void DeleteEmptyRows()
        {
            int maxWidth = 0;
            int maxDepth = 0;
            //Detect max width of the crossword puzzle
            for(int i=0; i<l1.Count; i++)
            {
                if(l1[i].hAlign == true)
                {
                    string g1 = l1[i].word;
                    int k22 = l1[i].word.Length;
                    int xCoordinate = l1[i].x - 1;
                    int m1 = k22 + xCoordinate;
                    if(maxWidth == 0 || maxWidth < m1)
                    {
                        maxWidth = l1[i].x + l1[i].word.Length;
                    }
                }
            }
            maxWidth = maxWidth + 2;

            //Detect max height of the puzzle
            for (int yy = 0; yy < l1.Count; yy++)
            {
                if (l1[yy].hAlign == false)
                {
                    string g1 = l1[yy].word;
                    int o1 = g1.Length;
                    int o2 = l1[yy].y;
                    int currDepth = o1 + o2;
                    
                    if(maxDepth == 0 || maxDepth < currDepth)
                    {
                        maxDepth = currDepth;
                    }
                }
            }
            maxDepth = maxDepth + 2;

            //If more than 1 empty rows are found, delete those leaving 1 row behind
            //to improve readability
            bool allCellsNull = true;
            int rowCnt = 0;
            int lastNullRowCnt = 0;
            List<DataRow> rowsToBeDeleted = new List<DataRow>();
            foreach(DataRow dr in dt.Rows)
            {
                for (int m1 = 0; m1 < maxWidth; m1++)
                {
                    if(!String.IsNullOrEmpty(dr[m1].ToString()))
                    {
                        allCellsNull = false;
                        break;
                    }
                }
                if (allCellsNull == true)
                {
                    if (lastNullRowCnt == (dt.Rows.IndexOf(dr) - 1))
                    {
                        //Leave atleast one empty row for better readability
                        rowsToBeDeleted.Add(dr);    
                   }
                    lastNullRowCnt = dt.Rows.IndexOf(dr);
                }
                allCellsNull = true;
                if (rowCnt > maxDepth)
                {
                    break;
                }
                rowCnt++;
            }
            //Now that the empty rows are detected, delete those from the datatable
            for (int j1 = 0; j1 < rowsToBeDeleted.Count - 1; j1++)
            {
                dt.Rows.Remove(rowsToBeDeleted[j1]);
            }

            //Remove null/empty columns
            while (dt.Columns.Count > maxWidth)
            {
                dt.Columns.RemoveAt(maxWidth);
            }

            //Remove null/empty rows
            maxDepth = (maxDepth - rowsToBeDeleted.Count) + 2;
            while (dt.Rows.Count > maxDepth)
            {
                dt.Rows.RemoveAt(maxDepth);
            }
        }


        private void placeT(theT _theT, int x, int y)
        {
            //Moves the enire T structure to a desired co-ordinate system
            placeWordHoriz2(_theT.horzW, x, y);
            placeWordVert2(_theT.vertW, x + _theT.horizIndex, y - _theT.vertIndex);
        }


        /// <summary>
        /// Detects if the word can be placed horizontally or not
        /// Checks to figure out if space is available or not
        /// </summary>
        /// <param name="theWord"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool placeWordHoriz2(string theWord, int x, int y)
        {
            bool isPlaceable = true;
            //Get the row of the datatable where this horizontal word needs to be placed
            DataRow dr = dt.Rows[y];
            //Get the length of the horizontal word that needs to be placed
            int l = theWord.Length;
            //The spots on the datarow either needs to be empty or exact same character as the new word about to fall in place
            for(int g1=0; g1<l; g1++)
            {
                string gridChar = dt.Rows[y][g1 + x].ToString();//dr[g1 + x].ToString();
                string incomingChar = theWord.ToCharArray()[g1].ToString();
                if((String.IsNullOrEmpty(gridChar)) || (gridChar == incomingChar))
                {
                    //Do nothing
                    //TODO: Log here
                }
                else
                {
                    //If its detected that the word can not be placed, break and return
                    isPlaceable = false;
                    break;
                }
            }
            if(isPlaceable)
            {
                //Spot is still available. Place the word successfully
                placeWordHoriz(theWord, x, y);
            }
            return isPlaceable;
        }


        private bool placeWordVert2(string theWord, int x, int y)
        {
            bool isPlaceable = true;
            //Get the column of the datatable where this vertical word needs to be placed
            //DataColumn dc = (DataColumn)dt.Rows[y][x];
            //DataRow dr = dt.Rows[y];
            //Get the length of the verticl word that needs to be placed
            int l = theWord.Length;
            //The spots on the datacolumn either needs to be empty or exact same character as the new word about to fall in place
            for (int g1 = 0; g1 < l; g1++)
            {
                string gridChar = dt.Rows[g1 + y][x].ToString();   //dc[g1 + y].ToString();
                string incomingChar = theWord.ToCharArray()[g1].ToString();
                if ((String.IsNullOrEmpty(gridChar)) || (gridChar == incomingChar))
                {
                    //Do nothing
                }
                else
                {
                    isPlaceable = false;
                    break;
                }
            }
            if (isPlaceable)
            {
                placeWordVert(theWord, x, y);
            }
            return isPlaceable;
        }

        /// <summary>
        /// Takes two words as inputs, the first parameter is vertical word and the other is horizontal
        /// If there are more than 1 common letters between the 2 words, the intersection runs on the leftmost letter of the 
        /// horizantal word. Returns an object of theT
        /// </summary>
        /// <param name="Vword1"></param>
        /// <param name="Hword2"></param>
        /// <returns></returns>
        private theT DetectT(string Vword1, string Hword2)
        {
            //If there's atleast a single common letter, a T found
            theT _theT = new theT();
            char[] h1 = Vword1.ToCharArray();
            char[] h2 = Hword2.ToCharArray();
            //There can be more than 1 common letters between any 2 words
            char[] h1h2 = h1.Intersect(h2).ToArray();

            int oldIndex = 0;   //HWord2 index
            string x = "";
            if (h1h2.Count() > 0)
            {
                for (int i = 0; i < h1h2.Count(); i++)
                {
                    int index = Hword2.IndexOf(h1h2[i]);
                    if (i == 0)
                    {
                        oldIndex = index;
                        x = h1h2[i].ToString();
                    }
                    else if (oldIndex > index)
                    {
                        //Pick the left most index of the horizantal word
                        oldIndex = index;
                        x = h1h2[i].ToString();
                    }
                }

                int i2 = Vword1.IndexOfAny(x.ToCharArray());  //Vword1 Index
                char c = x.ToCharArray()[0]; //The intersecting character

                //Populate the T object
                _theT.horizIndex = oldIndex;
                _theT.horzW = Hword2;
                _theT.vertIndex = i2;
                _theT.vertW = Vword1;
                _theT.intersection = c;
                return _theT;
            }
            //No T formation possible. Not a single character/letter in common
            return null;
        }

        /// <summary>
        /// returns the location details of any word in the datatable or main grid
        /// </summary>
        /// <param name="word1"></param>
        /// <returns></returns>
        private location findWord(string word1)
        {
            location l = l1.Where(m => m.word == word1).SingleOrDefault();
            return l;
        }

        private void placeWordVert(string word, int x, int y)
        {
            //Place the word in the datatable vertically
            for (int i3 = 0; i3 < word.ToCharArray().Length; i3++)
            {
                dt.Rows[y + i3][x] = word.ToCharArray()[i3];
            }
            //Store the main grid or datatable location coordinates
            if (findWord(word) == null)
            {
                location l = new location();
                l.word = word;
                l.hAlign = false;
                l.x = x;
                l.y = y;
                l1.Add(l);
            }
        }

        private void placeWordHoriz(string word, int x, int y)
        {
            //Place the word in the datatable horizontally
            for (int i2 = 0; i2 < word.ToCharArray().Length; i2++)
            {
                dt.Rows[y][x + i2] = word.ToCharArray()[i2];
            }
            //Store the main grid or datatable location coordinates
            if (findWord(word) == null)
            {
                location l = new location();
                l.word = word;
                l.hAlign = true;
                l.x = x;
                l.y = y;
                l1.Add(l);
            }
        }

        private void PrintPuzzleToCSV(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            File.WriteAllText("FinalCrossword.csv", sb.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "")
                abb = textBox1.Text.Split(',');
            CreateCrossword();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PrintPuzzleToCSV(dt);
        }
    }

    /// <summary>
    /// The location object saves information where exactly any word of the crossword is 
    /// located in the big grid. The x and y coordinate indicates the relative position
    /// in the main grid. The hAlign field tells the respective alignment of the word
    /// meaning if it is horizontally or vertically positioned in the main grid
    /// </summary>
    public class location
    {
        //the word
        public string word { get; set; }
        //x cordinate in the main big grid where the crossword puzzle is laid out
        public int x { get; set; }
        //y cordinate in the main big grid where the crossword puzzle is laid out
        public int y { get; set; }
        //Is true if the word is horizontally laid out else false
        public bool hAlign = true;
    }

    /// <summary>
    /// Saves all the details of 2 intersecting words forming a T
    /// Example of theT
    /// Horiz Word = planet
    /// Vert Word = earth
    /// Intersecting letter = a
    /// Horiz Index = 2
    /// Vert Index = 1
    /// </summary>
    public class theT
    {
        //The word which will be displayed vertically, top-to-bottom
        public string vertW { get; set; }
        //The word which will be displayed horizontally, left-to-right
        public string horzW { get; set; }
        //The index of the intersecting letter in the vertical word starting from top first letter
        //The index count begins with 0
        public int vertIndex { get; set; }
        //The index of the intersecting letter in the horizontal word starting from left first letter
        //The index count begins with 0
        public int horizIndex { get; set; }
        //The common intersecting letter
        public char intersection { get; set; }
    }

}
