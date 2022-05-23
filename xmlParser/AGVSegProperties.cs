using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmlParser
{
    class AGVSegProperties
    {
        string id = null;
        public string ID { get { return id; } set { id = value; } }

        string startPoint = null;
        public string StartPoint { get { return startPoint; } set { startPoint = value; } }

        string endPoint = null;
        public string EndPoint { get { return endPoint; } set { endPoint = value; } }

        string travelTime = null;
        public string TravelTime { get { return travelTime; } set { travelTime = value; } }

        string pathLength = null;
        public string PathLength { get { return pathLength; } set { pathLength = value; } }

        string flexible = null;
        public string Flexible { get { return flexible; } set { flexible = value; } }

        
        List<PlcAttrInfo> plcAttrInfo = null;
        public List<PlcAttrInfo> PlcAttrInfo { get { return plcAttrInfo; } set { plcAttrInfo = value; } }


        List<CarrierTypesInfo> carrierTypesInfo = null;
        public List<CarrierTypesInfo> CarrierTypesInfo { get { return carrierTypesInfo; } set { carrierTypesInfo = value; } }


        List<CoordInfo> coordInfos = null;
        public List<CoordInfo> CoordInfos { get { return coordInfos; } set { coordInfos = value; } }
    }

    class PlcAttrInfo
    {

    }
    
    class CarrierTypesInfo
    {

    }

    class CoordInfo
    {
        //string id = null;
        //public string ID { get { return id; } set { id = value; } }

        string coordx;
        public string CoordX { get { return coordx; } set { coordx = value; } }

        string coordy;
        public string CoordY { get { return coordy; } set { coordy = value; } }

        string angle;
        public string Angle { get { return angle; } set { angle = value; } }

        string speed;
        public string Speed { get { return speed; } set { speed = value; } }

        string refPntSpeed;
        public string RefPntSpeed { get { return refPntSpeed; } set { refPntSpeed = value; } }

        string rotationDir;
        public string RotationDir { get { return rotationDir; } set { rotationDir = value; } }

        string rotationSpeed;
        public string RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }

        string factor;
        public string Factor { get { return factor; } set { factor = value; } }


    }

}
