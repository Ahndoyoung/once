using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Once_v2_2015.Class
{
    public class OleDB
    {
        public static readonly string connPath = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\CafeContents.accdb";

        public static void CheckDB()
        {
            OleDbConnection conn = new OleDbConnection(connPath);
            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                // mdb가 없는 경우
                CreateDB();
            }
            finally
            {
                conn.Close();
            }
        }

        private static void CreateDB()
        {
            // 파일 생성
            try
            {
                Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
                if (objClassType != null)
                {
                    object obj = Activator.CreateInstance(objClassType);
                    obj.GetType().InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { connPath });

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }

                // 테이블 생성
                OleDbConnection conn = new OleDbConnection(connPath);
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    
                    // 결제 정보
                    cmd.CommandText =
                        "CREATE TABLE RECEIPT([RECEIPT_NUM] identity primary key, [RECEIPT_DATE] char, [RECEIPT_TYPE] char, [RECEIPT_DISCOUNT] int DEFAULT 0, [RECEIPT_SUBTOTAL] int, [RECEIPT_AMOUNT] int)";
                    cmd.ExecuteNonQuery();
                    // 판매 (메뉴) 정보
                    cmd.CommandText =
                        "CREATE TABLE SALE([SALE_NUM] IDENTITY PRIMARY KEY, [MENU_NAME] CHAR, [MENU_TEMP] CHAR, [MENU_SIZE] CHAR, [MENU_WHIP] CHAR DEFAULT F, [MENU_PRICE] INT, [SALE_QUANTITY] INT, [RECEIPT_NUM] INT)";
                    cmd.ExecuteNonQuery();
                }
                catch(Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
            catch
            {
            }
        }

        public static void NonQuery(string query)
        {
            OleDbConnection conn = new OleDbConnection(connPath);
            OleDbCommand cmd = new OleDbCommand(query,conn);

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
