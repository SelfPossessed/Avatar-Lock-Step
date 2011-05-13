﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace MyGame {
	public class Player : BasePlayer {
		public string Name;
        //public int ping; //should calculate it weighted instead
	}

    [RoomType("Avatar")]
	public class GameCode : Game<Player> {
        //private DateTime pingStart;
        //private const int turnLength = 150; //ms
        //private const int minUpdateTime = 50; //ms
        private const int turnLength = 75; //ms
        private const int minUpdateTime = 35; //ms
        private Player p1, p2;
        private Queue<Message> q1 = new Queue<Message>(), q2 = new Queue<Message>(); //need to synchronize, not sure how
        private System.Object lockThis = new System.Object(); //synchonization purposes

		// This method is called when an instance of your the game is created
		public override void GameStarted() {
			// anything you write to the Console will show up in the 
			// output window of the development server
			Console.WriteLine("Game is started: " + RoomId);
            
            /*
			// This is how you setup a timer
			AddTimer(delegate {
                Broadcast("Ping"); //should only ask for ping of players in gamestate, check game object
                pingStart = DateTime.Now;
				// code here will code every 100th millisecond (ten times a second)
			}, 10000);
            */
            
			/*
			// Debug Example:
			// Sometimes, it can be very usefull to have a graphical representation
			// of the state of your game.
			// An easy way to accomplish this is to setup a timer to update the
			// debug view every 250th second (4 times a second).
			AddTimer(delegate {
				// This will cause the GenerateDebugImage() method to be called
				// so you can draw a grapical version of the game state.
				RefreshDebugView(); 
			}, 250);
            */
		}

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {
			// this is how you send a player a message
			player.Send("hello");

			// this is how you broadcast a message to all players connected to the game
			Broadcast("UserJoined", player.Id);
		}
        
		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {
			Broadcast("UserLeft", player.Id);
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {
                case "C": //should be "" or " " for speed purposes later
                    /*
                    lock (lockThis) {
                        if (player == p1)
                            q1.Enqueue(message);
                        else if (player == p2)
                            q2.Enqueue(message);

                        if (q1.Count > 0 && q2.Count > 0) {
                            p1.Send(q2.Dequeue());
                            p2.Send(q1.Dequeue());
                        }
                    }
                    */
                    
                    if (player == p1)
                        p2.Send(message);
                    else if (player == p2)
                        p1.Send(message);
                    //Console.WriteLine(message);
                    
                    break;
				case "Ready":
                    if (p1 == null) {
                        p1 = player;
                    }else if(p2 == null) {
                        p2 = player;
                        //Broadcast("Start", turnLength, minUpdateTime);
                        p1.Send("Start", true, turnLength, minUpdateTime);
                        p2.Send("Start", false, turnLength, minUpdateTime);
                    }
                    break;
                /*
                case "Ping":
                    //int ping = (int)new TimeSpan(DateTime.Now.Ticks - pingStart.Ticks).TotalMilliseconds;
                    //player.ping = ping;
                    //Console.WriteLine(ping.ToString());
                    break;
                */
                case "Poke":
                    player.Send("Poke");
                    break;
			}
		}

		Point debugPoint;

		// This method get's called whenever you trigger it by calling the RefreshDebugView() method.
		public override System.Drawing.Image GenerateDebugImage() {
			// we'll just draw 400 by 400 pixels image with the current time, but you can
			// use this to visualize just about anything.
			var image = new Bitmap(400,400);
			using(var g = Graphics.FromImage(image)) {
				// fill the background
				g.FillRectangle(Brushes.Blue, 0, 0, image.Width, image.Height);

				// draw the current time
				g.DrawString(DateTime.Now.ToString(), new Font("Verdana",20F),Brushes.Orange, 10,10);

				// draw a dot based on the DebugPoint variable
				g.FillRectangle(Brushes.Red, debugPoint.X,debugPoint.Y,5,5);
			}
			return image;
		}

		// During development, it's very usefull to be able to cause certain events
		// to occur in your serverside code. If you create a public method with no
		// arguments and add a [DebugAction] attribute like we've down below, a button
		// will be added to the development server. 
		// Whenever you click the button, your code will run.
		[DebugAction("Play", DebugAction.Icon.Play)]
		public void PlayNow() {
			Console.WriteLine("The play button was clicked!");
		}

		// If you use the [DebugAction] attribute on a method with
		// two int arguments, the action will be triggered via the
		// debug view when you click the debug view on a running game.
		[DebugAction("Set Debug Point", DebugAction.Icon.Green)]
		public void SetDebugPoint(int x, int y) {
			debugPoint = new Point(x,y);
		}
	}
}