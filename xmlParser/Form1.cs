using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft;

namespace xmlParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            getSegmentInfo();
            originStart = "1";
            originEnd = "2";
            getArrayofRoute(originStart, originEnd);
            Debug.WriteLine("Done");
        }

        #region Astar algorithm

        string originStart = null;
        string originEnd = null;


        Dictionary<string, List<string>> aStarInfo = new Dictionary<string, List<string>>();

        //NodeID, {F Score, G Score, H Score, Parent Node}
        Dictionary<string, List<string>> closedList = new Dictionary<string, List<string>>();
        //NodeID, {F Score, G Score, H Score, Parent Node}
        Dictionary<string, List<string>> openedList = new Dictionary<string, List<string>>();
        // Astar 알고리즘 루트 어레이 최종 산출 변수
        List<string> arrayOfAGVRoute;
        void parsingSeginfoToAstarinfo()
        {
            for (int i = 0; i < agvSegProperties.Count; i++)
            {
                List<string> info = new List<string>();
                info.Add(agvSegProperties[i].StartPoint);
                info.Add(agvSegProperties[i].EndPoint);
                info.Add(agvSegProperties[i].TravelTime);
                info.Add(agvSegProperties[i].PathLength);
                aStarInfo.Add(agvSegProperties[i].ID, info);
            }
        }

        void getArrayofRoute(string startPoint, string endPoint)
        {
            // start point, end point 존재하는지 체크
                parsingSeginfoToAstarinfo();
            if (aStarInfo.ContainsKey(endPoint) && aStarInfo.ContainsKey(startPoint))
            {

                int isCMPLTArrayLoopstep = 0;
                bool isCMPLTArrayLoop = true;
                while (isCMPLTArrayLoop)
                {
                    switch (isCMPLTArrayLoopstep)
                    {
                        case 0:
                            // 출발 노드 정보를 closedList에 담는다.
                            closedList.Add(startPoint, new List<string>() { "0", "0", "0", "" });
                            isCMPLTArrayLoopstep = 1;
                            break;
                        case 1:
                            // 출발 Node와 연결된 Node의 정보를 Open List에 추가한다.
                            var nodelist = aStarInfo.Values.ToList();
                            for (int i = 0; i < nodelist.Count; i++)
                            {
                                if (nodelist[i].ToArray()[0] == startPoint)
                                {
                                    double gscore = double.Parse(nodelist[i].ToArray()[2]);
                                    double hscore = calcHeuristic(nodelist[i].ToArray()[1], endPoint);
                                    double fscore = hscore + gscore;

                                    //openlist중복경우 위 fscore적은걸로 바꾸기
                                    if (openedList.FirstOrDefault(x => x.Key == nodelist[i].ToArray()[1]).Key != null)
                                    {
                                        if (double.Parse(openedList.FirstOrDefault(x => x.Key == nodelist[i].ToArray()[1]).Value[0]) >= fscore)
                                        {
                                            openedList.Remove(nodelist[i].ToArray()[1]);
                                            openedList.Add(nodelist[i].ToArray()[1], new List<string>() { fscore.ToString(), gscore.ToString(), hscore.ToString(), nodelist[i].ToArray()[0] });
                                        }
                                    }
                                    else
                                    {
                                        openedList.Add(nodelist[i].ToArray()[1], new List<string>() { fscore.ToString(), gscore.ToString(), hscore.ToString(), nodelist[i].ToArray()[0] });
                                    }
                                }
                            }
                            isCMPLTArrayLoopstep = 2;
                            break;
                        case 2:
                            // case1에서 open list에 추가된 정보 중 Fscore가 가장 낮은 node를 close list에 추가한다.

                            // 단일 루프일때
                            if (openedList.Count == 1)
                            {
                                closedList.Add(openedList.FirstOrDefault().Key, openedList.FirstOrDefault().Value);
                                openedList.Remove(openedList.FirstOrDefault().Key);
                                startPoint = closedList.LastOrDefault().Key;
                                if (closedList.LastOrDefault().Key == endPoint)
                                {
                                    isCMPLTArrayLoop = false;
                                    arrayOfAGVRoute = closedList.Keys.ToList();
                                }
                                isCMPLTArrayLoopstep = 1;
                            }
                            // 분기가 있을때
                            else
                            {
                                //동일한 key 있을떄 처리해야됨
                                int isSameNodeLoopStep = 0;
                                bool isSameNodeLoop = true;
                                string openListKey = null;
                                while (isSameNodeLoop)
                                {
                                    switch (isSameNodeLoopStep)
                                    {
                                        case 0:
                                            var opendlistValueToList = openedList.Values.ToList();
                                            List<double> temp = new List<double>();
                                            for (int i = 0; i < openedList.Count; i++)
                                            {
                                                temp.Add(double.Parse(openedList.Values.ToArray()[i][0]));
                                            }
                                            int idx = temp.FindIndex(x => x == temp.Min());
                                            openListKey = openedList.FirstOrDefault(x => x.Value == opendlistValueToList[idx]).Key;

                                            if (closedList.FirstOrDefault(x => x.Key == openListKey).Key == null)
                                            {
                                                isSameNodeLoopStep = 1;
                                            }
                                            else
                                            {
                                                isSameNodeLoopStep = 2;
                                            }
                                            break;
                                        case 1:
                                            //동일한 key 없을때
                                            closedList.Add(openedList.FirstOrDefault(x => x.Key == openListKey).Key, openedList.FirstOrDefault(x => x.Key == openListKey).Value);
                                            openedList.Remove(openedList.FirstOrDefault(x => x.Key == openListKey).Key);
                                            startPoint = closedList.LastOrDefault().Key;

                                            isSameNodeLoop = false;
                                            break;
                                        case 2:
                                            openedList.Remove(openedList.FirstOrDefault(x => x.Key == openListKey).Key);
                                            isSameNodeLoopStep = 0;
                                            break;
                                    }
                                }

                                if (closedList.LastOrDefault().Key == endPoint)
                                {
                                    isCMPLTArrayLoop = false;
                                    List<string> wakan = new List<string>();

                                    bool isFinishGetEndNodeLoop = true;
                                    int isFinishGetEndNodeLoopstep = 0;
                                    string nodeKey = null;
                                    string nodeParent = null;

                                    while (isFinishGetEndNodeLoop)
                                    {
                                        switch (isFinishGetEndNodeLoopstep)
                                        {
                                            case 0:
                                                nodeKey = closedList.LastOrDefault().Key;
                                                wakan.Add(nodeKey);
                                                isFinishGetEndNodeLoopstep = 1;
                                                break;
                                            case 1:
                                                nodeParent = closedList.FirstOrDefault(x => x.Key == nodeKey).Value.ToArray()[3];
                                                isFinishGetEndNodeLoopstep = 2;
                                                break;
                                            case 2:
                                                nodeKey = closedList.FirstOrDefault(x => x.Key == nodeParent).Key;
                                                if (nodeKey != originStart)
                                                {
                                                    wakan.Add(nodeKey);
                                                    isFinishGetEndNodeLoopstep = 1;
                                                }
                                                else
                                                {
                                                    wakan.Add(nodeKey);
                                                    wakan = Enumerable.Reverse(wakan).ToList();
                                                    isFinishGetEndNodeLoop = false;
                                                }
                                                break;

                                        }

                                    }

                                    arrayOfAGVRoute = wakan;
                                }

                                isCMPLTArrayLoopstep = 1;
                            }
                            break;
                    }

                }
                foreach (var item in arrayOfAGVRoute)
                {
                    listBox1.Items.Add(item);

                }

            }
            else
            {
                MessageBox.Show("Point doesn't exit");
            }
        }


        double calcHeuristic( string startPoint, string endPoint)
        {
            var nodelist = aStarInfo.Values.ToList();
            bool findStartXY = false;
            bool findEndXY = false;
            int startX = 0;
            int startY = 0;
            int endX = 0;
            int endY = 0;
            for (int i = 0; i < nodelist.Count; i++)
            {
                if (nodelist[i].ToArray()[0] == startPoint)
                {
                    var _key = aStarInfo.FirstOrDefault(x => x.Value == nodelist[i]).Key;
                    int idx = agvSegProperties.FindIndex(a => a.ID.Contains(_key));
                    startX = Int32.Parse(agvSegProperties[idx].CoordInfos[0].CoordX);
                    startY = Int32.Parse(agvSegProperties[idx].CoordInfos[0].CoordY);
                    findStartXY = true;
                    if (findEndXY && findStartXY == true)
                    {
                        break;
                    }

                }
                if (nodelist[i].ToArray()[0] == endPoint)
                {
                    var _key = aStarInfo.FirstOrDefault(x => x.Value == nodelist[i]).Key;
                    int idx = agvSegProperties.FindIndex(a => a.ID.Contains(_key));
                    endX = Int32.Parse(agvSegProperties[idx].CoordInfos[0].CoordX);
                    endY = Int32.Parse(agvSegProperties[idx].CoordInfos[0].CoordY);
                    findEndXY = true;
                    if (findEndXY && findStartXY == true)
                    {
                        break;
                    }
                }
            }
            double underLength = Math.Pow(((Math.Abs(startX))-Math.Abs(endX)),2);
            double height = Math.Pow(((Math.Abs(startY)) - Math.Abs(endY)), 2);
            return Math.Sqrt(underLength+ height);
        }

        #endregion

        #region MapData XML Parsing


        string commonID = null;
        List<CoordInfo> coordInfos = new List<CoordInfo>();
        List<AGVSegProperties> agvSegProperties = new List<AGVSegProperties>();
        
        void getSegmentInfo()
        {
            XmlDocument testDocument = new XmlDocument();
            testDocument.Load("XIAN.xml");

            XmlNodeList testXMLNodeList = testDocument.GetElementsByTagName("Segment");
            string Id = null;
            string StartPoint = null;
            string EndPoint = null;
            string Weight = null;
            string Length = null;

            foreach (XmlNode item in testXMLNodeList)
            {
                commonID = item.Attributes["Id"].Value;
                Id = item.Attributes["Id"].Value;
                StartPoint = item.Attributes["StartPoint"].Value;
                EndPoint = item.Attributes["EndPoint"].Value;
                Weight = item.Attributes["Weight"].Value;
                Length = item.Attributes["Length"].Value;

                partsparsing(item.OuterXml);

                agvSegProperties.Add(new AGVSegProperties { ID = Id, StartPoint = StartPoint, EndPoint = EndPoint, TravelTime = Weight, PathLength = Length, CoordInfos = coordInfos });
            }

        }

        void partsparsing(string input)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);
            
            XmlNodeList carrierTypesNodeList = doc.GetElementsByTagName("CarrierTypes");
            XmlNodeList partsNodeList = doc.GetElementsByTagName("Parts");

            string CoordX = null;
            string CoordY = null;
            string Angle = null;
            string Speed = null;
            string RefPntSpeed = null;
            string RotationDir = null;
            string RotationSpeed = null;
            List<CoordInfo> internalCoordinfo = new List<CoordInfo>();


            foreach (XmlNode partsNodeListitem in partsNodeList)
            {
               XmlNodeList child = partsNodeListitem.ChildNodes;
                foreach (XmlNode childitem in child)
                {

                    switch (childitem.LocalName)
                    {
                        case "PartPoint":
                            CoordX = childitem.Attributes["CoordX"].Value;
                            CoordY = childitem.Attributes["CoordY"].Value;
                            Angle = childitem.Attributes["Angle"].Value;
                            internalCoordinfo.Add(new CoordInfo { CoordX = CoordX, CoordY = CoordY, Angle = Angle });
                            coordInfos = internalCoordinfo;
                            break;

                        case "PartRotation":
                            RotationDir = childitem.Attributes["RotationDir"].Value;
                            RotationSpeed = childitem.Attributes["RotationSpeed"].Value;
                            internalCoordinfo.Add(new CoordInfo { RotationDir = RotationDir, RotationSpeed = RotationSpeed });
                            coordInfos = internalCoordinfo;
                            break;

                        //case "PartLine":
                        //    bool a = nullCheck(childitem.Attributes["RefPntSpeed"]);
                        //    Speed = childitem.Attributes["Speed"].Value;
                        //    if(a == true)
                        //    RefPntSpeed = childitem.Attributes["RefPntSpeed"].Value;
                        //    internalCoordinfo.Add(new CoordInfo { Speed = Speed, RefPntSpeed = RefPntSpeed });
                        //    coordInfos = internalCoordinfo;

                        //    break;

                        default:
                            break;
                    }

                }
            }
        }

        bool nullCheck(XmlAttribute name)
        {
            try
            {
                if (name.Value == null)
                {
                    throw new ArgumentNullException(nameof(name));
                    return false;
                }
                else
                    return true;

            }
            catch
            {
                return false;
            }
        }
        #endregion
    }

}