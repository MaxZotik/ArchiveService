using ArchiveService.Class.Database.MS_SQL;
using ArchiveService.Class.Database.MS_SQL.Triggers;
using ArchiveService.Class.Enums;
using ArchiveService.Class.Log;
using ArchiveService.Class.Modbus_Work.VastModbus;
using ArchiveService.Class.Patterns;
using ArchiveService.Class.ServerTCP;
using ArchiveService.Class.VASTModbusTCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        //private Server server = new Server();
        private Thread[] threads;
        private int countThread = MyModbusTCP.MyModbusTCPList.Count;
        private MyModbusTCPmode myModbusTCPmode = new MyModbusTCPmode();

        /// <summary>
        /// Метод запускаеи службу Windows
        /// </summary>
        /// <param name="args">Может принимать массив строк</param>
        protected override void OnStart(string[] args)
        {
            Thread threadServer = new Thread(ReadClientMode);
            threadServer.Start();

            Thread threadClient = new Thread(ReadClient);                   
            threadClient.Start();

            Thread threadTriggerAverage = new Thread(TriggersAverage);
            threadTriggerAverage.Start();

            Thread threadTriggerDelete = new Thread(TriggersDelete);
            threadTriggerDelete.Start();

            Thread threadTriggerDeletePeak = new Thread(TriggersDeletePeak);
            threadTriggerDeletePeak.Start();

            Thread threadsWrite = new Thread(WriteLog);
            threadsWrite.Start();
        }

        /// <summary>
        /// Метод остановки службы Windows
        /// </summary>
        protected override void OnStop()
        {
            new Loggings().WriteLogAdd($"Служба \"Archive Service\" остановлена!", StatusLog.Action);
            Thread threadsWrite = new Thread(WriteLog);
            threadsWrite.Start();
        }

        protected void ReadClientMode()
        {
            while (true)
            {
                Stopwatch timer = Stopwatch.StartNew();

                Thread thread = new Thread(myModbusTCPmode.MVKtempOneMode)
                {
                    Name = "ServiceMode",
                    Priority = ThreadPriority.Highest
                };

                thread.Start();
                thread.Join();

                timer.Stop();
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);

                int timeSleep = 485;

                if (time < timeSleep)
                    timeSleep -= time;

                Thread.Sleep(timeSleep);
            }
        }

        protected void ReadClient()
        {        
            while (true)
            {
                Stopwatch timer = Stopwatch.StartNew();

                threads = new Thread[countThread];

                for (int i = 0; i < countThread; i++)
                {

                    threads[i] = new Thread(MyModbusTCP.MyModbusTCPList[i].MVKtempOneMulti)
                    {
                        Name = "Service: " + (i + 1).ToString(),
                        Priority = ThreadPriority.Highest
                    };
                    threads[i].Start();
                }

                foreach (Thread th in threads)
                {
                    th.Join();
                }

                Thread threadTriggerInsert = new Thread(new TriggerInsert().TriggerMSSQLInsert)
                {
                    Name = "TriggerMSSQLInsert",
                    Priority = ThreadPriority.Normal
                };
                threadTriggerInsert.Start();

                threadTriggerInsert.Join();

                timer.Stop();
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);

                int timeSleep = 425;

                if (time < timeSleep)
                    timeSleep -= time;

                Thread.Sleep(timeSleep);

            }
        }   
        
        protected void TriggersAverage()
        {
            while (true)
            {
                Thread threadTriggerAverage = new Thread(new TriggerAverage().TriggerMSSQLAverage)
                {
                    Name = "TriggerMSSQLAverage",
                    Priority = ThreadPriority.Normal
                };
                threadTriggerAverage.Start();

                Thread.Sleep(475);
            }
        }

        protected void TriggersDelete()
        {
            while (true)
            { 
                Thread threadTriggerDelete = new Thread(new TriggerDelete().TriggerMSSQLDelete)
                {
                    Name = "TriggerMSSQLDelete",
                    Priority= ThreadPriority.Lowest
                };
                threadTriggerDelete.Start();

                Thread.Sleep(420000);     //7 минут

            }
        }

        protected void TriggersDeletePeak()
        {
            while (true)
            {
                Thread threadTriggerDeletePeak = new Thread(new TriggerDelete().TriggerMSSQLDeletePeak)
                {
                    Name = "TriggerMSSQLDelete",
                    Priority= ThreadPriority.Lowest
                };
                threadTriggerDeletePeak.Start();

                Thread.Sleep(510000);     //8,5 минут
            }
        }

        protected void WriteLog()
        {
            while (true)
            {
                Stopwatch timer = Stopwatch.StartNew();

                Thread threadWriteCrate = new Thread(Loggings.WriteLogFile)
                {
                    Name = "Service WriteLogFile",
                    Priority = ThreadPriority.Normal
                };
                threadWriteCrate.Start();
                threadWriteCrate.Join();

                timer.Stop();
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);

                int timeSleep = 990;

                if (time < timeSleep)
                    timeSleep -= time;

                Thread.Sleep(timeSleep);
            }
        }
        #region Метод для запуска службы как консольное приложение (для отладки приложения)
        /// <summary>
        /// Метод для запуска службы как консольное приложение (для отладки приложения)
        /// </summary>
        /// <param name="args">Массив строк</param>
        //internal void TestStartupAndStop(string[] args)
        //{
        //    this.OnStart(args);
        //    Console.ReadLine();
        //    this.OnStop();
        //}

        #endregion
    }
}
