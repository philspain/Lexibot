using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;

using LexibotLogic.Reddit;
using LexibotData;
using LexibotData.Models;
using LexibotData.DataAccess;
using LexibotLogic.Parsers;
using LexibotLogic.DIContainer;

namespace LexibotService
{
    public partial class Service1 : ServiceBase
    {
        WordParser _wordParser;
        RedditBot _redditBot;
        Thread _lexibotThread;
        Logger _logger;

        public Service1() :
            this(DependencyFactory.Resolve<Logger>(),
                 DependencyFactory.Resolve<WordParser>())
        {

        }

        public Service1(Logger logger, WordParser wordParser)
        {
            _logger = logger;
            _wordParser = wordParser;
            InitializeComponent();
        }

        public void DoWork()
        {
            try
            {
                _redditBot = new RedditBot();
                _redditBot.Run();
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\r\n" + ex.InnerException + "\r\n" + ex.Source + "\r\n" + ex.StackTrace;
                _logger.LogError(message);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _lexibotThread = new Thread(new ThreadStart(DoWork));

                _lexibotThread.Start();
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\r\n" + ex.InnerException + "\r\n" + ex.Source + "\r\n" + ex.StackTrace;
                _logger.LogError(message);
            }
        }

        protected override void OnStop()
        {
            _lexibotThread.Abort();
        }
    }
}
