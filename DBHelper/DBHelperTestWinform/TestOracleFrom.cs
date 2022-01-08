using DBUtil;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace DBHelperTestWinform
{
    public partial class TestOracleFrom : Form
    {
        private IDBHelper dbHelper = ServiceHelper.Get<IDBHelper>(() => new DBHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), DBType.Oracle));

        public TestOracleFrom()
        {
            InitializeComponent();
        }

        private void TestOracleFrom_Load(object sender, EventArgs e)
        {

        }

        private void Log(string msg)
        {
            this.BeginInvoke(new Action(() =>
            {
                textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + msg + "\r\n");
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Log("开始");
                try
                {
                    List<CARINFO_MERGE> list = CacheUtil.TryGetValue<List<CARINFO_MERGE>>("CARINFO_MERGE", () =>
                    {
                        using (var session = dbHelper.GetSession())
                        {
                            string sql = "select * from CARINFO_MERGE where rownum<20000";
                            LogTimeUtil logTime = new LogTimeUtil();
                            List<CARINFO_MERGE> result = session.FindListBySql<CARINFO_MERGE>(sql);
                            Log(logTime.LogTime("耗时"));
                            return result;
                        }
                    });
                    Log("结束");
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\r\n" + ex.StackTrace);
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Log("开始");
                try
                {
                    using (var session = dbHelper.GetSession())
                    {
                        string sql = "select * from CARINFO_MERGE where rownum<20000";
                        LogTimeUtil logTime = new LogTimeUtil();
                        List<CARINFO_MERGE> result = session.FindListBySql<CARINFO_MERGE>(sql);
                        Log(logTime.LogTime("耗时"));
                    }
                    Log("结束");
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\r\n" + ex.StackTrace);
                }
            });
        }
    }
}
