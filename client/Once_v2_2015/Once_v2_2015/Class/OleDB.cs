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
        private static string connPath = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\CafeContents.accdb";

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

                    // 카테고리 정보
                    cmd.CommandText =
                        "CREATE TABLE MENU_CATEGORY([MENU_CATEGORY_NUM] IDENTITY PRIMARY KEY, [MENU_CATEGORY_NAME] VARCHAR, [MENU_CATEGORY_DELETE] CHAR DEFAULT N)";
                    cmd.ExecuteNonQuery();
                    // 메뉴 정보 (외래키 : MENU_CATEGORY_NUM)
                    cmd.CommandText =
                        "CREATE TABLE MENU([MENU_NUM] IDENTITY PRIMARY KEY, [MENU_NAME] VARCHAR, [MENU_SIZE] CHAR, [MENU_TEMP] CHAR, [MENU_WHIP] CHAR DEFAULT N, [MENU_PRICE] INT, [MENU_DELETE] CHAR DEFAULT N, [MENU_CATEGORY_NUM] INT)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText =
                        "ALTER TABLE MENU ADD CONSTRAINT MENU_CATEGORY_FK FOREIGN KEY (MENU_CATEGORY_NUM) REFERENCES MENU_CATEGORY(MENU_CATEGORY_NUM)";
                    cmd.ExecuteNonQuery();
                    // 결제 정보
                    cmd.CommandText =
                        "CREATE TABLE RECEIPT([RECEIPT_NUM] identity primary key, [RECEIPT_DATE] DATETIME, [RECEIPT_TYPE] char, [RECEIPT_DISCOUNT] int DEFAULT 0, [RECEIPT_SUBTOTAL] int, [RECEIPT_AMOUNT] int)";
                    cmd.ExecuteNonQuery();
                    // 판매 (메뉴) 정보 (외래키 : MENU_NUM, RECEIPT_NUM)
                    cmd.CommandText =
                        "CREATE TABLE SALE([SALE_NUM] IDENTITY PRIMARY KEY, [MENU_NUM] INT, [RECEIPT_NUM] INT, [SALE_QUANTITY] INT)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText =
                        "ALTER TABLE SALE ADD CONSTRAINT MENU_FK FOREIGN KEY (MENU_NUM) REFERENCES MENU(MENU_NUM)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText =
                        "ALTER TABLE SALE ADD CONSTRAINT RECEIPT_FK FOREIGN KEY (RECEIPT_NUM) REFERENCES RECEIPT(RECEIPT_NUM)";
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
    }
}
