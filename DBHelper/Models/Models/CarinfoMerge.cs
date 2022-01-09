using System;
using System.Collections.Generic;
using System.Linq;
using DBUtil;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DBTable("CARINFO_MERGE")]
    public partial class CarinfoMerge
    {

        /// <summary>
        /// 
        /// </summary>
        [IsId]
        [IsDBField]
        public int? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("MODIFY_TIME")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("LICENSE_NO")]
        public string LicenseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_PLATE_COLOR")]
        public string CarPlateColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string Brand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_TYPE")]
        public string CarType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("PASSENGER_LEVEL")]
        public string PassengerLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_COLOR")]
        public string CarColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ENG_NO")]
        public string EngNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("FRAME_NO")]
        public string FrameNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_IDENTITY_CODE")]
        public string CarIdentityCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("SEAT_NO")]
        public decimal? SeatNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_TONNAGE")]
        public decimal? CarTonnage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("FUEL_TYPE")]
        public string FuelType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ENG_POWER")]
        public decimal? EngPower { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("LEAVE_FACTORY_TIME")]
        public DateTime? LeaveFactoryTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BUY_CAR_TIME")]
        public DateTime? BuyCarTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("SETTLE_TIME")]
        public DateTime? SettleTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public int? Wheelbase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_LENGTH")]
        public int? CarLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_HEIGHT")]
        public int? CarHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_WIDTH")]
        public int? CarWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("DRIVING_WAY")]
        public string DrivingWay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TRANSFORM_LICENSE_NO")]
        public string TransformLicenseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("OWNER_NAME")]
        public string OwnerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BUSINESS_LICENSE_NO")]
        public string BusinessLicenseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("GRANT_ORG")]
        public string GrantOrg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BEGIN_TIME")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("END_TIME")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BUSINESS_SCOPE")]
        public string BusinessScope { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_OPERATE_SITUATION")]
        public string CarOperateSituation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CAR_TECHNOLOGY_LEVEL")]
        public string CarTechnologyLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("DRIVE_RECORDER")]
        public int? DriveRecorder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public int? Locator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("FUEL_EXAM_TIME")]
        public DateTime? FuelExamTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("FIRST_GRANT_TIME")]
        public DateTime? FirstGrantTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string Anchored { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ADMINISTRATIVE_DIVISION_CODE")]
        public string AdministrativeDivisionCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ORG_CODE")]
        public string OrgCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("OWNER_ADDRESS")]
        public string OwnerAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ECONOMIC_TYPE")]
        public string EconomicType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("LEGAL_REPRESENTATIVE")]
        public string LegalRepresentative { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ID_CARD_NO")]
        public string IdCardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string Telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("OWNER_TYPE")]
        public string OwnerType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("OWNER_ABBREVIATION")]
        public string OwnerAbbreviation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CHECK_COMPANY")]
        public string CheckCompany { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("VEHICLE_NO")]
        public string VehicleNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CHECK_PERSON")]
        public string CheckPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string Manafacture { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("PRODUCT_NAME")]
        public string ProductName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("VHCL_COMPANY")]
        public string VhclCompany { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("VHCL_TYPE")]
        public string VhclType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CHASSIS_TYPE")]
        public string ChassisType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("DRIVE_TYPE")]
        public string DriveType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TYRE_SIZE")]
        public string TyreSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ENGINE_TYPE1")]
        public string EngineType1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TOTAL_MASS")]
        public decimal? TotalMass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TRALIER_MASS")]
        public decimal? TralierMass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("WHEEL_MASS")]
        public decimal? WheelMass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TRUCK_VOL")]
        public decimal? TruckVol { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("SEAT_SUM")]
        public int? SeatSum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public int? Length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public int? Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public int? High { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BOX_LENGTH")]
        public int? BoxLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BOX_WIDTH")]
        public int? BoxWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BOX_HIGH")]
        public int? BoxHigh { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CURB_WEIGHT")]
        public int? CurbWeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("SIZE_EXCEED")]
        public decimal? SizeExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("BOARD_EXCEED")]
        public decimal? BoardExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CURB_WEIGHT_EXCEED")]
        public decimal? CurbWeightExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CHECK_COM_OPTION")]
        public string CheckComOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("MANAGER_COM_PERSON")]
        public string ManagerComPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("MANAGER_COM_OPTION")]
        public string ManagerComOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TEST_DATE")]
        public DateTime? TestDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("IS_TRANS")]
        public string IsTrans { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("DISTRICT_ID")]
        public string DistrictId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("ENGINE_TYPE")]
        public string EngineType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("TELEPHONE_1")]
        public string Telephone1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("CHECK_STATE")]
        public string CheckState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("SEAT_EXCEED")]
        public decimal? SeatExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IsDBField("VHCL_COLOR")]
        public string VhclColor { get; set; }

    }
}
