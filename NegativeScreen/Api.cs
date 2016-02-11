using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NegativeScreen
{
	class Api : IDisposable
	{
		private OverlayManager _overlayManager;
		private HttpListener _listener;
		public Api(OverlayManager overlayManager)
		{
			_overlayManager = overlayManager;
			_listener = new HttpListener();
			_listener.IgnoreWriteExceptions = true;
			_listener.Prefixes.Add(Configuration.Current.ApiListeningUri);
			_listener.Start();
			Thread t = new Thread(ThreadLoop) { Name = "Api Listener Thread" };
			t.Start();
		}

		private void ThreadLoop()
		{
			int previousErrorCount = 0;
			const int maxSuccessiveErrorCount = 10;
			while (_listener.IsListening)
			{
				try
				{
					var context = _listener.GetContext();
					if (context.Request.HttpMethod == "POST")
					{
						const int maxRead = 1024;
						byte[] buffer = new byte[maxRead];
						var read = context.Request.InputStream.Read(buffer, 0, maxRead);
						string body = Encoding.UTF8.GetString(buffer, 0, read);
						switch (body)
						{
							case "TOGGLE":
								_overlayManager.Toggle();
								WriteResponse(context.Response, "Ok.");
								break;
							case "ENABLE":
								_overlayManager.Enable();
								WriteResponse(context.Response, "Ok.");
								break;
							case "DISABLE":
								_overlayManager.Disable();
								WriteResponse(context.Response, "Ok.");
								break;
							default:
								const string setEffectCommand = "SET ";
								if (body.StartsWith(setEffectCommand))
								{
									var effectName = body.Substring(setEffectCommand.Length);
									var effectExists = _overlayManager.TrySetColorEffectByName(effectName);
									if (effectExists)
									{
										WriteResponse(context.Response, "Ok.");
									}
									else
									{
										WriteResponse(context.Response, "Effect not found.");
									}
									break;
								}
								else
								{
									WriteResponse(context.Response, "Unrecognized command.");
								}
								break;
						}
					}
					else
					{
						WriteResponse(context.Response, $"NegativeScreen {Application.ProductVersion}.");
					}
					context.Response.Close();
					previousErrorCount = 0; // Reset the error count upon successful request.
				}
				catch
				{
#if DEBUG
					Debugger.Break();
#endif
					previousErrorCount++;
					if (previousErrorCount >= maxSuccessiveErrorCount)
					{
						// Something is wrong, take the api down.
						Exit();
					}
				}
			}
		}

		private void WriteResponse(HttpListenerResponse response, string body)
		{
			response.KeepAlive = false;
			response.SendChunked = false;
			var buffer = Encoding.UTF8.GetBytes($"{body}\r\n");
			response.ContentLength64 = buffer.Length;
			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
		}


		public void Exit()
		{
			_listener.Close();
		}

		public void Dispose()
		{
			Exit();
		}
	}
}
