using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SimulacaoInterfaceRemote
{
    public partial class Form1 : Form
    {
        private enum MaquinaEstado {Desligado = -1, Principal, initConfig, endConfig, Take, AV };
        private int estado;
        private enum MaquinaAV { Audio = 0, Video, Audio_Video };
        private int estado_AV;
        private int entrada_anteriorConfig;
        private int saida_anteriorConfig;
        private int[] entradasAudio = new int[100];
        private int[] entradasVideo = new int[100];
        public Form1()
        {

            InitializeComponent();
            for (int i = 0; i < 100; i++)
            {
                entradasAudio[i] = -1;
                entradasVideo[i] = -1;
            }
            estado = (int)MaquinaEstado.Desligado;
            
        }

        #region Inicialização

        private void switchOn_MouseDown(object sender, MouseEventArgs e)
        {
            if (switchOn.Value == 0)
            {
                switchOn.Value = 1;
                Teste_Todos_Botoes();
                MessageBox.Show("Detectou Matriz AUDIO e VIDEO!");
                btAV.BackColor = Color.Yellow;
                btEnable.BackColor = Color.Red;
                estado = (int)MaquinaEstado.Principal;
                estado_AV = (int)MaquinaAV.Audio_Video;
                entrada_anteriorConfig = -1;
            }
            else 
            { 
                switchOn.Value = 0;
                Reset();
                timerEnable.Enabled = false;
                btEnable.BackColor = Color.Red;
                btAV.BackColor = Color.White;
                btEnable.BackColor = Color.White;
                estado = (int)MaquinaEstado.Desligado;
            }
        }

        private void Reset()
        {
            timerAV_YEL.Enabled = false;
            estado = (int)MaquinaEstado.Principal;
            estado_AV = (int)MaquinaAV.Audio_Video;
            Write_Todos_Botoes();
            btAV.BackColor = Color.Yellow;
            btConfig.BackColor = Color.White;
            btTake.BackColor = Color.White;
            entrada_anteriorConfig = -1;
            Update();
        }

        private void Teste_Todos_Botoes()
        {
            for (int i = groupBox3.Controls.Count-1; i >=0; i--)
            {
                if (groupBox3.Controls[i].GetType() == typeof(Button))
                {
                    if (groupBox3.Controls[i].Enabled)
                    {
                        groupBox3.Controls[i].BackColor = Color.Lime;
                        groupBox3.Controls[i].Update();
                        Thread.Sleep(150);
                        groupBox3.Controls[i].BackColor = Color.White;
                        groupBox3.Controls[i].Update();
                    }
                }
            }
            for (int i = groupBox3.Controls.Count - 1; i >= 0; i--)
            {
                if (groupBox3.Controls[i].GetType() == typeof(Button))
                {
                    if (groupBox3.Controls[i].Enabled)
                    {
                        groupBox3.Controls[i].BackColor = Color.Red;
                        groupBox3.Controls[i].Update();
                        Thread.Sleep(150);
                        groupBox3.Controls[i].BackColor = Color.White;
                        groupBox3.Controls[i].Update();
                    }
                }
            }
        }

        #endregion

        #region Enable

        private void timerEnable_Tick(object sender, EventArgs e)
        {
            Reset();
            timerEnable.Enabled = false;
            btEnable.BackColor = Color.Red;
            
            
        }

        private void btEnable_Click(object sender, EventArgs e)
        {
            if (estado == (int)MaquinaEstado.Desligado) return;
            if (btEnable.BackColor == Color.Lime)
            {
                Reset();
                timerEnable.Enabled = false;
                btEnable.BackColor = Color.Red;
            }
            else
            {
                timerEnable.Enabled = false;
                timerEnable.Enabled = true;
                estado = 0;
                btEnable.BackColor = Color.Lime;
                Write_Todos_Botoes();
            }
            
        }

        #endregion
        
        #region AV

        private void btAV_Click(object sender, EventArgs e)
        {
            if (estado == (int)MaquinaEstado.Desligado || btEnable.BackColor!=Color.Lime) return;
            if (estado == (int)MaquinaEstado.Principal)
            {
                Write_Todos_Botoes();
                switch (estado_AV)
                {
                    case (int)MaquinaAV.Audio:
                        {
                            estado_AV = (int)MaquinaAV.Video;
                            btAV.BackColor = Color.Lime;
                            break;
                        }
                    case (int)MaquinaAV.Video:
                        {
                            estado_AV = (int)MaquinaAV.Audio_Video;
                            btAV.BackColor = Color.Yellow;
                            break;
                        }
                    case (int)MaquinaAV.Audio_Video:
                        {
                            estado_AV = (int)MaquinaAV.Audio;
                            btAV.BackColor = Color.Red;
                            break;
                        }
                }
            }
            else if (estado == (int)MaquinaEstado.AV || estado == (int)MaquinaEstado.initConfig)
            {
                if (estado == (int)MaquinaEstado.AV)estado = (int)MaquinaEstado.initConfig;
                timerAV_YEL.Enabled = false;
                switch (estado_AV)
                {
                    case (int)MaquinaAV.Audio:
                        {
                            estado_AV = (int)MaquinaAV.Video;
                            btAV.BackColor = Color.Lime;
                            break;
                        }
                    case (int)MaquinaAV.Video:
                        {
                            estado_AV = (int)MaquinaAV.Audio;
                            btAV.BackColor = Color.Red;
                            break;
                        }
                    case (int)MaquinaAV.Audio_Video:
                        {
                            estado_AV = (int)MaquinaAV.Audio;
                            btAV.BackColor = Color.Red;
                            break;
                        }
                }

            }
        }
        #endregion

        #region Teclas 1 a 32

        private void teclas_Click(object sender, EventArgs e)
        {
            if (estado == (int)MaquinaEstado.Desligado || btEnable.BackColor != Color.Lime) return;
            Control controlSender = new Control();
            foreach (Control c in groupBox3.Controls)
            {
                if (sender == c) controlSender=c;
            }
            timerEnable.Enabled = false;
            timerEnable.Enabled = true;
            if (estado == (int)MaquinaEstado.Principal) //Selecionada a Saida do Update
            {
                Write_Todos_Botoes();
                switch (estado_AV)
                {
                    case (int)MaquinaAV.Audio:
                        {
                            controlSender.BackColor = Color.Yellow;
                            if (entradasAudio[groupBox3.Controls.IndexOf(controlSender)] != -1) groupBox3.Controls[entradasAudio[groupBox3.Controls.IndexOf(controlSender)]].BackColor = Color.Red;
                            break;
                        }
                    case (int)MaquinaAV.Video:
                        {
                            controlSender.BackColor = Color.Yellow;
                            if (entradasVideo[groupBox3.Controls.IndexOf(controlSender)] != -1) groupBox3.Controls[entradasVideo[groupBox3.Controls.IndexOf(controlSender)]].BackColor = Color.Lime;
                            break;
                        }
                    case (int)MaquinaAV.Audio_Video:
                        {
                            controlSender.BackColor = Color.Yellow;
                            if (entradasAudio[groupBox3.Controls.IndexOf(controlSender)] != -1) groupBox3.Controls[entradasAudio[groupBox3.Controls.IndexOf(controlSender)]].BackColor = Color.Red;
                            if (entradasVideo[groupBox3.Controls.IndexOf(controlSender)] != -1) groupBox3.Controls[entradasVideo[groupBox3.Controls.IndexOf(controlSender)]].BackColor = Color.Lime;
                            break;
                        }
                }
            }
            else if (estado == (int)MaquinaEstado.initConfig) //Selecionada a Saida do Config
            {
                Write_Todos_Botoes();
                switch (estado_AV)
                {
                    case (int)MaquinaAV.Audio:
                        {
                            controlSender.BackColor = Color.Yellow;
                            if (entradasAudio[groupBox3.Controls.IndexOf(controlSender)] != -1)
                            {
                                groupBox3.Controls[entradasAudio[groupBox3.Controls.IndexOf(controlSender)]].BackColor = Color.Red;
                                entrada_anteriorConfig = groupBox3.Controls.IndexOf(groupBox3.Controls[entradasAudio[groupBox3.Controls.IndexOf(controlSender)]]);
                            }
                            break;
                        }
                    case (int)MaquinaAV.Video:
                        {
                            controlSender.BackColor = Color.Yellow;
                            if (entradasVideo[groupBox3.Controls.IndexOf(controlSender)] != -1)
                            {
                                groupBox3.Controls[entradasVideo[groupBox3.Controls.IndexOf(controlSender)]].BackColor = Color.Lime;
                                entrada_anteriorConfig = groupBox3.Controls.IndexOf(groupBox3.Controls[entradasVideo[groupBox3.Controls.IndexOf(controlSender)]]);
                            }
                            break;
                        }
                }
                saida_anteriorConfig = groupBox3.Controls.IndexOf(controlSender);
                estado = (int)MaquinaEstado.endConfig;
            }
            else if (estado == (int)MaquinaEstado.endConfig) //Selecionada a Entrada do Config apagando a entrada anterior
            {
                switch (estado_AV)
                {
                    case (int)MaquinaAV.Audio:
                        {
                            controlSender.BackColor = Color.Red;
                            controlSender.Update();
                            entradasAudio[saida_anteriorConfig] = groupBox3.Controls.IndexOf(controlSender);
                            if (entrada_anteriorConfig != -1)
                            {
                                groupBox3.Controls[entrada_anteriorConfig].BackColor = Color.White;
                                groupBox3.Controls[entrada_anteriorConfig].Update();
                            }
                            break;
                        }
                    case (int)MaquinaAV.Video:
                        {
                            controlSender.BackColor = Color.Lime;
                            controlSender.Update();
                            entradasVideo[saida_anteriorConfig] = groupBox3.Controls.IndexOf(controlSender);
                            if (entrada_anteriorConfig != -1)
                            {
                                groupBox3.Controls[entrada_anteriorConfig].BackColor = Color.White;
                                groupBox3.Controls[entrada_anteriorConfig].Update();
                            }
                            break;
                        }
                }
                groupBox3.Controls[saida_anteriorConfig].BackColor = Color.Lime;
                groupBox3.Controls[saida_anteriorConfig].Update();
                btConfig.BackColor = Color.Lime;
                btConfig.Update();
                Thread.Sleep(1000);
                btConfig.BackColor = Color.Yellow;
                btConfig.Update();
                Write_Todos_Botoes();
                estado = (int)MaquinaEstado.initConfig;
                entrada_anteriorConfig = -1;
            }
        }

        private void Write_Todos_Botoes()
        {
            foreach (Control b in groupBox3.Controls)
            {
                if (b.GetType() == typeof(Button))
                {
                    if (b.Enabled)
                    {
                        b.BackColor = Color.White;

                    }
                }

            }
        }

        #endregion

        #region Config

        private void btConfig_Click(object sender, EventArgs e)
        {
            if (estado == (int)MaquinaEstado.Desligado || btEnable.BackColor != Color.Lime) return;
            if (estado == (int)MaquinaEstado.Principal)
            {
                timerEnable.Enabled = false;
                timerEnable.Enabled = true;
                Write_Todos_Botoes();
                btConfig.BackColor = Color.Yellow;
                btConfig.Update();
                btAV.BackColor = Color.Yellow;
                timerAV_YEL.Enabled = true;
                timerEntradaToggle.Enabled = true;
                estado = (int)MaquinaEstado.AV;
            }
            else if (estado == (int)MaquinaEstado.initConfig || estado == (int)MaquinaEstado.endConfig)
            {
                timerEnable.Enabled = false;
                timerEnable.Enabled = true;
                Reset();
                timerEntradaToggle.Enabled = false;
            }
        }

        private void timerAV_YEL_Tick(object sender, EventArgs e)
        {
            if (btAV.BackColor == Color.Yellow) btAV.BackColor = Color.White;
            else btAV.BackColor = Color.Yellow;
        }

        private void timerEntradaToggle_Tick(object sender, EventArgs e)
        {
            if (entrada_anteriorConfig == -1) return;
            
            if(estado_AV==(int)MaquinaAV.Audio)
            {
                if (groupBox3.Controls[entrada_anteriorConfig].BackColor == Color.Red) groupBox3.Controls[entrada_anteriorConfig].BackColor = Color.White;
                else groupBox3.Controls[entrada_anteriorConfig].BackColor = Color.Red;
            } 
            else if (estado_AV == (int)MaquinaAV.Video)
            {
                if (groupBox3.Controls[entrada_anteriorConfig].BackColor == Color.Lime) groupBox3.Controls[entrada_anteriorConfig].BackColor = Color.White;
                else groupBox3.Controls[entrada_anteriorConfig].BackColor = Color.Lime;
            }
            
        }

        #endregion





    }
}
