using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PerformanceTest
{
    public partial class Form1 : Form
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        private Random _rnd = new Random();
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Log
        private void Log(string log)
        {
            if (!this.IsDisposed)
            {
                string msg = DateTime.Now.ToString("mm:ss.fff") + " " + log + "\r\n\r\n";

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        textBox1.AppendText(msg);
                    }));
                }
                else
                {
                    textBox1.AppendText(msg);
                }
            }
        }
        #endregion

        #region 删除
        private void button5_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Log("删除 开始");
                using (var session = DBHelper.GetSession())
                {
                    session.DeleteByCondition<SysUser>(string.Format("id>=12"));
                }
                Log("删除 完成");
            });
        }
        #endregion

        #region 测试批量修改
        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                List<SysUser> userList = m_SysUserDal.GetList("select t.* from sys_user t where t.id > 20");

                foreach (SysUser user in userList)
                {
                    user.Remark = "测试修改用户" + _rnd.Next(1, 10000);
                    user.UpdateUserid = "1";
                    user.UpdateTime = DateTime.Now;
                }

                Log("批量修改 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;
                m_SysUserDal.Update(userList);
                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("批量修改 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 测试批量添加
        private void button4_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                m_SysUserDal.Get("1"); //预热

                List<SysUser> userList = new List<SysUser>();
                for (int i = 1; i <= 1000; i++)
                {
                    SysUser user = new SysUser();
                    user.UserName = "testUser";
                    user.RealName = "测试插入用户";
                    user.Password = "123456";
                    user.CreateUserid = "1";
                    userList.Add(user);
                }

                Log("批量添加 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;
                m_SysUserDal.Insert(userList);
                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("批量添加 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 测试循环修改
        private void button7_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                List<SysUser> userList = m_SysUserDal.GetList("select t.* from sys_user t where t.id > 20");

                foreach (SysUser user in userList)
                {
                    user.Remark = "测试修改用户" + _rnd.Next(1, 10000);
                    user.UpdateUserid = "1";
                    user.UpdateTime = DateTime.Now;
                }

                Log("循环修改 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;
                foreach (SysUser user in userList)
                {
                    m_SysUserDal.Update(user);
                }
                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("循环修改 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 测试循环添加
        private void button6_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                m_SysUserDal.Get("1"); //预热

                List<SysUser> userList = new List<SysUser>();
                for (int i = 1; i <= 1000; i++)
                {
                    SysUser user = new SysUser();
                    user.UserName = "testUser";
                    user.RealName = "测试插入用户";
                    user.Password = "123456";
                    user.CreateUserid = "1";
                    userList.Add(user);
                }

                Log("循环添加 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;
                foreach (SysUser user in userList)
                {
                    m_SysUserDal.Insert(user);
                }
                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("循环添加 完成，耗时：" + time + "秒");
            });
        }
        #endregion

    }
}
