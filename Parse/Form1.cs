using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
// esto
// esto tambien


namespace Parse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ArchivoOrigen = new OpenFileDialog();
            ArchivoOrigen.Filter = "Archivo log|*.log";

            DialogResult Resultado = ArchivoOrigen.ShowDialog();
            if (Resultado == DialogResult.OK)
            {
                textBox1.Text = ArchivoOrigen.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog CarpetaDestino = new FolderBrowserDialog();
            CarpetaDestino.ShowNewFolderButton = true;
            if (CarpetaDestino.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = CarpetaDestino.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();

        }

        private static string RemoveUnquotedWhiteSpaces(string text)
        {
            string result = String.Empty;
            var parts = text.Split('"');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0) result += Regex.Replace(parts[i], " ", "%");
                else result += String.Format("\"{0}\"", parts[i]);
            }
            return result;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            String Linea;
           
            List<String> Campos = new List<String>();
            Dictionary<String, String> CamposDiccionario = new Dictionary<string, string>();
            int cuenta = 0;
            char delimitador = '%';
            String[] campo_valor;
            String registro = "";
            string archivoAParsear = textBox1.Text;
            string detalle = textBox2.Text + "\\detalle.txt";
            string encabezado = textBox2.Text + "\\encabezado.txt";
            string archivoSalida = textBox2.Text + "\\Parseado_" + Path.GetFileName(archivoAParsear);
            
            using (StreamWriter salida1 = new StreamWriter(encabezado))
            using (StreamWriter salida2 = new StreamWriter(detalle))
            using (StreamReader Archivo = File.OpenText(archivoAParsear))
            {
                backgroundWorker1.ReportProgress(cuenta, "Parseando columnas....");

                while ((Linea = Archivo.ReadLine()) != null)
                {
                    Campos.Clear();

                    //Crea la lista Campos a partir de la linea del archivo.  Cada linea es de la forma "campo1=valor1 campo2=valor2"
                    //La funcion RemoveUnquotesWhiteSpaces elimina los espacios que estan FUERA de los campos encerrados en comillas
                    Campos.AddRange(RemoveUnquotedWhiteSpaces(Linea).Split(delimitador));

                    registro = "";
                    foreach (string columna in Campos)
                    {
                        campo_valor = columna.Split('=');
                        if (!CamposDiccionario.ContainsKey(campo_valor[0]))
                        {
                            CamposDiccionario.Add(campo_valor[0], campo_valor[1]);
                        }
                        else
                        {
                            CamposDiccionario[campo_valor[0]] = campo_valor[1];
                        }

                    }

                    foreach (var item in CamposDiccionario)
                    {
                        registro = registro + item.Value + ",";
                    }
                    registro = registro.TrimEnd(',');
                    salida2.WriteLine(registro);

                    cuenta++;
                    backgroundWorker1.ReportProgress(cuenta);

                    List<string> claves = new List<string>(CamposDiccionario.Keys);
                    for (int i = 0; i < claves.Count(); i++)
                    {
                        string clave = claves[i];
                        CamposDiccionario[clave] = "";
                    }


                }
                registro = "";
                foreach (var item in CamposDiccionario)
                {
                    registro = registro + item.Key + ",";
                }
                registro = registro.TrimEnd(',');
                salida1.WriteLine(registro);


                salida1.Close();
                salida2.Close();
                salida1.Dispose();
                salida2.Dispose();
                Archivo.Close();
                Archivo.Dispose();


            }

            File.Copy(encabezado, archivoSalida);

            using (StreamWriter salida = new StreamWriter(archivoSalida, append:true))
            using (StreamReader entrada = new StreamReader(detalle))
            {
                while ((Linea = entrada.ReadLine()) != null)
                {
                    salida.WriteLine(Linea);
                }

                salida.Close();
                entrada.Close();
                salida.Dispose();
                entrada.Dispose();

            }


            File.Delete(encabezado);
            File.Delete(detalle);

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                label3.Text = e.UserState.ToString();
            }

            textBox3.Text = e.ProgressPercentage.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
