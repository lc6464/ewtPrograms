using System;
using System.Timers;
using System.Net.Http;
using System.Threading.Tasks;

namespace EwtKeepLogin {
	internal class Program {
		static void Main(string[] args) {
			var cookies = "";
			if (args.Length > 1) {
				Console.WriteLine("Args > 1!");
				return;
			} else if (args.Length == 0) {
				Console.Write("Please enter your cookies: ");
				cookies = Console.ReadLine();
			} else if (args.Length == 1) {
				if (args[0] == "--help" || args[0] == "-h" || args[0] == "-?") {
					Console.WriteLine("EwtKeepLogin.exe \"you cookies...\"");
					return;
				}
				cookies = args[0];
			}
			var handler = new HttpClientHandler() { UseCookies = false };
			var hc = new HttpClient(handler) { BaseAddress = new Uri("https://web.ewt360.com/customerApi/api/studyprod/lessonCenter/getUserTimeRanking"), Timeout = new TimeSpan(0, 0, 5) };
			hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.124 Safari/537.36 Edg/102.0.1245.44");
			hc.DefaultRequestHeaders.Add("Cookie", cookies);

			using (var task = Send(hc)) {
				task.Wait();
				if (!task.Result) {
					return;
				}
			}

			using (var timer = new Timer() { Enabled = true, Interval = 5500 }) {
				timer.Elapsed += async (object sender, ElapsedEventArgs e) => { if (!await Send(hc)) { ((Timer)sender).Dispose(); } };
			}

			while (true) {
				var input = Console.ReadLine();
				if (input == "quit") {
					Console.WriteLine("Exit!");
					hc.Dispose();
					break;
				} else if (input == "clear") {
					Console.Clear();
				} else {
					Console.WriteLine("Enter \"quit\" then press \"Enter\" to exit.\r\nEnter \"clear\" then press \"Enter\" to clear console.");
				}
			}
		}

		/// <summary>
		/// 发送请求。
		/// </summary>
		/// <param name="hc">使用的 <see cref="HttpClient"/></param>
		/// <returns>如果失败，则返回 false，否则返回 true.</returns>
		static async Task<bool> Send(HttpClient hc) {
			try {
				var result = await hc.GetStringAsync("").ConfigureAwait(false);
				if (result.Contains("请重新登录")) {
					Console.WriteLine("Cookies 无效！");
					return false;
				}
				Console.WriteLine(result);
				return true;
			} catch (Exception e) {
				Console.Write("Error! Message: ");
				Console.WriteLine(e);
				return false;
			}
		}
	}
}