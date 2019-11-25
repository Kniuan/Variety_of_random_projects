using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Timers;

public class receiver : MonoBehaviour {
    public static bool isRunning = false;
    Socket sock1;
    Socket sock2;
    Byte[] b1 = new byte[10000];
    Byte[] b2 = new byte[10000];

    public CalibrationButtonScript CBS;

    public double alpha1 = 0;
    public double alpha1_0 = 0;
    public double alpha1_1 = 0;
    public double alpha1_2 = 0;
    public double alpha1_3 = 0;
    public double alpha2 = 0;
    public double alpha2_0 = 0;
    public double alpha2_1 = 0;
    public double alpha2_2 = 0;
    public double alpha2_3 = 0;

    public double delta1 = 0;
    public double delta1_0 = 1;
    public double delta1_1 = 0;
    public double delta1_2 = 0;
    public double delta1_3 = 0;
    public double delta2 = 0;
    public double delta2_0 = 1;
    public double delta2_1 = 0;
    public double delta2_2 = 0;
    public double delta2_3 = 0;

    public static bool OneOrTwo = false;
    public static bool win = false;
    public static double percentA = .5;
    private static int winTimer = 0;

    double AlphaP = 0;
    float brainBallTarget = 0;

    public GameObject brainBall;

    public float final1 = 1;
    public float final2 = 1;

    public Thread t1;
    public Thread tWin;


    void Start() {

        final1 = PlayerPrefs.GetFloat("Final1");
        final2 = PlayerPrefs.GetFloat("Final2");
        Debug.Log("Final 1 is: " + final1);
        Debug.Log("Final 2 is: " + final2);


        //networking connection and threading code

        IPEndPoint ipThingo1 = new IPEndPoint(IPAddress.Any, 12345);
        IPEndPoint ipThingo2 = new IPEndPoint(IPAddress.Any, 12346);
        sock1 = new Socket(IPAddress.Any.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        sock1.Bind(ipThingo1);
        sock2 = new Socket(IPAddress.Any.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        sock2.Bind(ipThingo2);
        isRunning = true;
        t1 = new Thread(new ThreadStart(receive));
        tWin = new Thread(new ThreadStart(winState));
        t1.Start();
        tWin.Start();
    }

    void Update () {
            float a = (float)AlphaP;
            if (a < 0.5f)
            {
                a = 0.5f;
            }
            int b = 0;
            if (OneOrTwo)
            {
                b = 1;
            }
            else
            {
                b = -1;
            }

            brainBallTarget = ((1 - a) * 14f) * b;
            Vector2 v = brainBall.transform.position;

        if (!System.Single.IsNaN(brainBallTarget))
        {
            brainBall.transform.Translate(Vector2.right * (brainBallTarget - v.x) * Time.deltaTime);
        }

        //Debug.Log(brainBallTarget + " " + OneOrTwo);
    }


    //receiver function to stream in and parse data from OpenBCI
    void receive()
    {
        while (isRunning)
        {
            sock1.Receive(b1);
            for (int i = 0; i < b1.Length; i++)
            {
                if ((int)b1[i] != 0)
                {
                    try
                    {
                        string s = System.Text.Encoding.UTF8.GetString(b1, 0, b1.Length);     //Convert the byte array into a String
                        s = s.Replace(Convert.ToChar(0x0).ToString(), "");      //Delete all the null chracters
                        s = s.Substring(27, s.Length - 29);
                        //Debug.Log(s);
                        ;
                        string[] ss = s.Split(',');
                        ss[0] = ss[0].Trim('[');
                        ss[5] = ss[5].Trim('[');
                        ss[10] = ss[10].Trim('[');
                        ss[15] = ss[15].Trim('[');
                        //Debug.Log(ss);
                        //Debug.Log(ss[2] + " " + ss[7] + " " + ss[12] + " " + ss[17] + "\n" + s);
                        //Trims the data up to be readable

                        alpha1_0 = Double.Parse(ss[2]); //All the alpha values
                        alpha1_1 = Double.Parse(ss[7]);
                        alpha1_2 = Double.Parse(ss[12]);
                        alpha1_3 = Double.Parse(ss[17]);
                        ///*
                        delta1_0 = Double.Parse(ss[0]); //All the delta values
                        delta1_1 = Double.Parse(ss[5]);
                        delta1_2 = Double.Parse(ss[10]);
                        delta1_3 = Double.Parse(ss[15]);
                        //*/
                        alpha1 = ((alpha1_0 + alpha1_1) / 2) / ((alpha1_2 + alpha1_3) / 2); //Compares back to front to get the correct alpha values and averages them too
                        delta1 = (delta1_0 + delta1_1 + delta1_2 + delta1_3) / 4; //Simply averages the delta values to get an overall value
                        Array.Clear(b1, 0, 10000);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("woah there pardner\n" + e.InnerException + "\n" + e.Message);
                        isRunning = false;
                    }
                }
            }

            sock2.Receive(b2);
            for (int i = 0; i < b2.Length; i++)
            {
                if ((int)b2[i] != 0)
                {
                    try
                    {
                        string s = System.Text.Encoding.UTF8.GetString(b2, 0, b2.Length);     //Convert the byte array into a String
                        //Debug.Log(s);
                        s = s.Replace(Convert.ToChar(0x0).ToString(), "");      //Delete all the null chracters
                        s = s.Substring(27, s.Length - 29);
                        string[] ss = s.Split(',');
                        ss[0] = ss[0].Trim('[');
                        ss[5] = ss[5].Trim('[');
                        ss[10] = ss[10].Trim('[');
                        ss[15] = ss[15].Trim('[');
                        //Debug.Log(ss[2] + " " + ss[7] + " " + ss[12] + " " + ss[17] + "\n" + s);
                        //Trims the data up to be readable

                        alpha2_0 = Double.Parse(ss[2]); //All the alpha values
                        alpha2_1 = Double.Parse(ss[7]);
                        alpha2_2 = Double.Parse(ss[12]);
                        alpha2_3 = Double.Parse(ss[17]);
                        ///*
                        delta2_0 = Double.Parse(ss[0]); //All the delta values
                        delta2_1 = Double.Parse(ss[5]);
                        delta2_2 = Double.Parse(ss[10]);
                        delta2_3 = Double.Parse(ss[15]);
                        //*/
                        alpha2 = ((alpha2_0 + alpha2_1) / 2) / ((alpha2_2 + alpha2_3) / 2); //Compares back to front to get the correct alpha values and averages them too
                        delta2 = (delta2_0 + delta2_1 + delta2_2 + delta2_3) / 4; //Simply averages the delta values to get an overall value
                        Array.Clear(b2, 0, 10000);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("woah there pardner\n" + e.InnerException + "\n" + e.Message);
                        isRunning = false;
                    }
                }
            }
            //Debug.Log("Alpha 1: " + alpha1 + ", Alpha 2: " + alpha2 + ". Inside recieve.\n");
        }
    }

    public double getAlpha1()
    {
        return this.alpha1;
    }

    public double getAlpha2()
    {
        return this.alpha2;
    }

    public double getDelta1()
    {
        return this.delta1;
    }

    public double getDelta2()
    {
        return this.delta2;
    }

    private static void addTimer(object source, ElapsedEventArgs e)
    {
        winTimer++;
        Debug.Log("Wintimer: " + winTimer); //++s an int to do timer related stuff
    }

    private static void beeTimer(object source, ElapsedEventArgs e)
    {

        if (percentA <= .8) {
            percentA += .1;
            Debug.Log("percentA: " + percentA);
        }

        else if (percentA >= .8 && percentA < .99)
        {
            percentA += .01;
            Debug.Log("percentA: " + percentA);
        }

        //Will count to .9, then will continue counting to .99, from which it will cease.

    }

    void winState()
    {
        
        double Alpha1Current = 0;
        double Alpha2Current = 0;
        double Delta1Current = 0;
        double Delta2Current = 0;
        percentA = .5;
        AlphaP = 0;

        System.Timers.Timer aTimer = new System.Timers.Timer(); //Handles the winTimer
        aTimer.Elapsed += new System.Timers.ElapsedEventHandler(addTimer);
        aTimer.Interval = 1000;

        System.Timers.Timer bTimer = new System.Timers.Timer(); //Handles the decaying percent
        bTimer.Elapsed += new System.Timers.ElapsedEventHandler(beeTimer);
        bTimer.Interval = 30000;
        bTimer.Enabled = true;
        //Declartions of variables

        while (isRunning)
        {
            //Debug.Log("Alpha 1 is : " + getAlpha1());
            //Debug.Log("Alpha 2 is : " + getAlpha2());
            Alpha1Current = (getAlpha1() / final1) - getDelta1();
            Alpha2Current = (getAlpha2() / final2) - getDelta2();
            //Debug.Log("Alpha 1: " + Alpha1Current + ", Alpha 2: " + Alpha2Current + ". Inside winState.\n");
            //This is where the point value of each player is done. Basically, getalpha gets the alpha from the 
            //reciever above, final is the calibration value, and getDelta gets the delta value from above to minus into the total value.

            if (Alpha1Current > Alpha2Current)
            {
                AlphaP = Alpha2Current / Alpha1Current;
                OneOrTwo = true;
            }
            else if(Alpha1Current < Alpha2Current) {
                AlphaP = Alpha1Current / Alpha2Current;
                OneOrTwo = false;
            }
            //Confirms who is ahead currently

            if (AlphaP <= percentA) //This is iis where the timer for when someone is winning is started and stopped as needed
            {
                aTimer.Enabled = true;
                //Debug.Log("Timer countdown");

                if (OneOrTwo && winTimer == 4)
                {
                    win = true;
                }
                if (!OneOrTwo && winTimer == 4)
                {
                    win = true;
                }

            }
            else
            {
                aTimer.Enabled = false;
                winTimer = 0;
                //Debug.Log("RESET!!!");
            }

            if (win) //Wins and closes everything out
            {
                aTimer.Stop();
                bTimer.Stop();
                winTimer = 0;
                
                t1.Abort();
                tWin.Abort();
                sock1.Close();
                sock2.Close();
                
                break;
            }
           
        }
        
    }

    public static void Exit() {
        isRunning = false;     //this doesn't seem to actually abort the thread
    }
}
