using Godot;
using System;
using System.Text;

public partial class Main : Node
{
    TextureRect Fondo;
    Control GameMenu, MainMenu;
    Panel Rec;
    Label r1, r2, r3, d1, Historial,acc;
    ScrollContainer sc;
    Button Talar, Quemar, Criar;
    int acciones = 2,_1=0,_2=0;
    [Export]
    public float _Madera = 150, _Arboles = 5000, _Poblacion = 5000;
    public float Madera = 150, Arboles = 5000, Poblacion = 5000;
    double fTime = 5;
    float SizeRec = 220;
    RandomNumberGenerator r = new RandomNumberGenerator();
    HttpRequest www, www01, www02;
    string url = "https://dev.gaxoslabs.ai/api/connect/v1", key = "WNilM5QXqozs8CSPLlgWujLCL8zL109QV9F3tS4K";
    string LastID = "";
    int rondas = 0;
    int TalarLvl = 1;
    int QuemarLvl = 1;
    int CriarLvl = 1;
    float pors;
    AudioStreamPlayer com;
    public override void _Ready()
    {
        www = GetNode<HttpRequest>("www");
        www01 = GetNode<HttpRequest>("www01");
        www02 = GetNode<HttpRequest>("www02");
        r.Randomize();
        Fondo = GetNode<TextureRect>("Fondo");
        MainMenu = GetNode<Control>("Menu");
        GameMenu = GetNode<Control>("Game");
        Rec = GameMenu.GetNode<Panel>("Rec");
        r1 = Rec.GetNode<Label>("VBoxContainer/Rec1");
        r2 = Rec.GetNode<Label>("VBoxContainer/Rec2");
        r3 = Rec.GetNode<Label>("VBoxContainer/Rec3");
        d1 = Rec.GetNode<Label>("VBoxContainer/Data1");
        Historial = Rec.GetNode<Label>("VBoxContainer/s/v/Historial");
        acc=GameMenu.GetNode<Label>("Player/Bar/axiones/Label2");
        Talar = GameMenu.GetNode<Button>("Player/Bar/axiones/HBoxContainer/Talar/TalarUp");
        Quemar = GameMenu.GetNode<Button>("Player/Bar/axiones/HBoxContainer/Quemar/QuemarUp");
        Criar = GameMenu.GetNode<Button>("Player/Bar/axiones/HBoxContainer/Criar/CriarUp");
        sc = Rec.GetNode<ScrollContainer>("VBoxContainer/s");
        MainMenu.Visible = true;
        GameMenu.Visible = false;
        com = GetNode<AudioStreamPlayer>("Comentario");
        //no olvidar
        //no olvidar
        RequestByPrompt("A forest with evil trees. The forest should be located near a waterfall, surrounded by dark mountains and be under twilight. The trees should seem strange and disturbing, as if they had a life of their own. The light should be dim and the image should convey a sense of mystery and terror.");
        //no olvidar
        //no olvidar
        var i = new Image();
        if (i.Load("user://IMG.PNG") == Error.Ok)
            Fondo.Texture = ImageTexture.CreateFromImage(i);
        else
        {
            i = ResourceLoader.Load<Image>("res://TEMP.PNG");
            Fondo.Texture = ImageTexture.CreateFromImage(i);
        }
        Comentario("En un mundo donde los arboles cobraron conciencia de vida y se preparan para atacar, la umanidad se defiende con achas y fuego alborde del colapso. Tu tarea será decidir que se debe hacer, todos te siguen y admiran. Buena suerte!");
    }
    public void RequestByPrompt(string prompt="")
    {
        string[] s = new string[] { "accept: application/json", "Authorization: Bearer " + key, "Content-Type: application/json" };
        var e=www.Request(url+ "/request", s,HttpClient.Method.Post, "{" +
                "\"generator\": \"gaxos-text-to-image\"," +
                "\"generator_parameters\" :" +
                "{" +
                    "\"prompt\": \""+prompt+"\"," +
                    "\"steps\": \"15\"," +
					"\"height\": \"288\"," +
					"\"width\": \"512\"" +
                "}" +
            "}");
        if (e != Error.Ok)
            GD.PrintErr("Request ERROR");
    }
    private void Resp(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result == (int)HttpRequest.Result.Success)
        {
            GD.Print(UTF8Encoding.UTF8.GetString(body));
            RequestByID(UTF8Encoding.UTF8.GetString(body));
        }
    }
    public void RequestByID(string id) {
        string[] s = new string[] { "accept: application/json", "Authorization: Bearer " + key};
        var e = www01.Request(url + "/request/" + id, s);
        LastID = id;
        if (e != Error.Ok)
            GD.PrintErr("Request ERROR");
    }
    private void Resp01(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result == (int)HttpRequest.Result.Success)
        {
            GD.Print(UTF8Encoding.UTF8.GetString(body));
            var j = Json.ParseString(UTF8Encoding.UTF8.GetString(body)).AsGodotDictionary<string, string>();
            if (j["status"].Contains("GENERATED"))
            {
                var ss = j["assets"].Split(",")[0].Split("\"", false);
                RequestDownload(ss[ss.Length - 1]);
            }
            else if (j["status"].Contains("PENDING"))
            {
                GD.Print("loop by" + LastID);
                RequestByID(LastID);
            }
            else
            {
                GD.PrintErr("Falla al generar id" + LastID);
            }
        }
    }
    public void RequestDownload(string id = "")
    {
        string[] s = new string[] { "accept: application/json", "Authorization: Bearer " + key};
        var e = www02.Request(url + "/download/"+ id, s);
        if (e != Error.Ok)
            GD.PrintErr("Request ERROR");
    }
    private void Resp02(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result == (int)HttpRequest.Result.Success)
        {
            var i = new Image();
            i.LoadPngFromBuffer(body);
            Fondo.Texture = ImageTexture.CreateFromImage(i);
            i.SavePng("user://IMG.PNG");
        }
    }
    public void TUp()
    {
        TalarLvl++;
        Talar.Visible = false;
        Quemar.Visible = false;
        Criar.Visible = false;
    }
    public void QUp()
    {
        QuemarLvl++;
        Talar.Visible = false;
        Quemar.Visible = false;
        Criar.Visible = false;
    }
    public void CUp()
    {
        CriarLvl++;
        Talar.Visible = false;
        Quemar.Visible = false;
        Criar.Visible = false;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (God.yaco.estado == EstadoGame.Jugando1)
        {
            fTime -= delta;
            if (fTime <= 0 || acciones <= 0 && God.yaco.estado != EstadoGame.Victoria)
            {
                var p = Poblacion / Arboles;
                God.yaco.estado = EstadoGame.Jugando2;
                fTime = r.RandiRange(2, 5);
                Historial.Text = "Turno de la Naturaleza\n"+ Historial.Text;
                if (rondas%4 == 0)
                {
                    Talar.Visible = true;
                    Quemar.Visible = true;
                    Criar.Visible = true;
                }
            }
        }
        else if (God.yaco.estado == EstadoGame.Jugando2 && God.yaco.estado != EstadoGame.Perdida)
        {
            fTime -= delta;
            if (fTime <= 0)
            {
                if (Arboles < Poblacion)
                    switch (++_2)
                    {
                        case 0:
                            Historial.Text = "Naturaleza PowerUp 1\nArboles +15%\n"+ Historial.Text;
                            Comentario("Power up de Arboles 1, aumentan su numero");
                            Arboles *= 1.15f;
                            break;
                        case 1:
                            Historial.Text = "Naturaleza PowerUp 2\nArboles +25%\n" + Historial.Text;
                            Arboles *= 1.25f;
                            Comentario("Power up de Arboles 2, aumentan mas su numero, preparate");
                            break;
                        case 2:
                            Historial.Text = "Naturaleza PowerUp 3\nPoblacion -15%\n" + Historial.Text;
                            Poblacion /= 1.15f;
                            Comentario("Power up de Arboles 3, nos quitaron amigos");
                            break;
                        case 3:
                            Historial.Text = "Naturaleza PowerUp 4\nPoblacion -25%\n" + Historial.Text;
                            Poblacion /= 1.25f;
                            Comentario("Ultimo Power up de Arboles, nos quitaron amigos y familia");
                            break;
                    }

                if (rondas % 2 == 0)
                {
                    var a = 200 * (int)((TalarLvl + QuemarLvl + CriarLvl) / 3f);
                    Arboles += a;
                    Historial.Text = "Arboles +" + a + "\n" + Historial.Text;
                    Comentario("Los arboles se protege, vienen sus "+a+" refuerzos");
                }
                else
                {
                    Poblacion -= Arboles * 0.2f;
                    Historial.Text = "Poblacion -"+ Arboles * 0.2f + "\n" + Historial.Text;
                    Comentario("Los arboles acabaron con "+ (int)(Arboles * 0.2f)+ ", No se rindan esta guerra sí se puede ganar");
                }
                God.yaco.estado = EstadoGame.Jugando1;
                fTime = 30;
                acciones = 2;
                Historial.Text = "Turno de los Humanos\n" + Historial.Text;
                rondas++;
            }
        }
        if (God.yaco.estado != EstadoGame.Victoria && God.yaco.estado != EstadoGame.Perdida)
        {

            if (Arboles <= 0)
            {
                God.yaco.estado = EstadoGame.Victoria;
                Historial.Text = "\nLos Humanos ganan\n" + Historial.Text;
                Comentario("Los Humanos ganan y ya pueden llorar a sus caidos en batalla");
            }
            else if (Poblacion <= 0)
            {
                God.yaco.estado = EstadoGame.Perdida;
                Historial.Text = "\nLos Arboles ganan\n" + Historial.Text;
                Comentario("Los Arboles ganan y esclavizan a la humanidad");
            }
            else
            {
                pors = Poblacion / Arboles;
            }
            //sc.ScrollVertical += 10;
            acc.Text = ((int)fTime) + " Acciones " + acciones;
        }
        r1.Text = "Persona/Arbol " + ((int)(pors * 100) / 100f).ToString()+"p/a";
        r2.Text = "Madera " + ((int)(Madera * 100) / 100f).ToString();
        r3.Text = "Poblacion " + ((int)(Poblacion * 100) / 100f).ToString();
        d1.Text = "Arboles " + ((int)(Arboles * 100) / 100f).ToString();
        if (Mathf.Abs(SizeRec - Rec.Size.X) > 3 && God.yaco.estado != EstadoGame.Menu)
            Rec.Size += (Vector2.Right * (SizeRec - Rec.Size.X)).Normalized() * 600 * (float)delta;
       

    }
    public void _on_talar_pressed()
    {
        if (God.yaco.estado == EstadoGame.Jugando1 && acciones > 0)
        {
            acciones--;
            var i=r.RandiRange(0, 100);
            if (i > 55)
            {
                Comentario("Caes en una emboscada y pierdes "+(int)(Arboles * 0.1f)+" guerreros");
                Poblacion -= Arboles * 0.1f;
                Historial.Text = "Poblacion -" + Arboles * 0.1f + "\n" + Historial.Text;
            }
            else if(i>25){
                Comentario("Los guerreros alzan las cabezas de enemigos caidos, pero la guerra continua");
            }
            Arboles -= Poblacion*0.2f;
            Madera += Poblacion * 0.2f * TalarLvl;
            Historial.Text = "Madera +" + Poblacion * 0.2f * TalarLvl + "\nArboles -"+ Poblacion * 0.2f + "\n" + Historial.Text;
        }
    }

    private void Comentario(string text)
    {
        OS.Execute("Voz.exe", new string[] { text, AppContext.BaseDirectory+"\\Comentario.mp3" }, null, true, true);
        var a=FileAccess.Open(AppContext.BaseDirectory + "\\Comentario.mp3",FileAccess.ModeFlags.Read);
        AudioStreamMP3 aud=new AudioStreamMP3();
        aud.Data = a.GetBuffer((long)a.GetLength());
        com.Stream = aud;
        a.Close();
        com.Play();
    }

    public void _on_quemar_pressed()
    {
        if (God.yaco.estado == EstadoGame.Jugando1 && acciones > 0)
        {
            acciones--;
            var ma = 100 * QuemarLvl;//fijo mejor a futuro
            Arboles -= ma;
            Historial.Text = "Arboles -"+ma+ "\n" + Historial.Text;
            Comentario("acabas con "+ma+" arboles, la humanidad aclama tus estrategias");
        }
    }
    public void _on_criar_pressed()
    {
        if (God.yaco.estado == EstadoGame.Jugando1 && acciones > 0)
        {
            acciones--;
            var m = -1;
            if (Madera >= 5000)
            {
                m = 500;
            }
            else
            if (Madera >= 2500)
            {
                m = 250;
            }
            else
            if (Madera >= 2000)
            {
                m = 200;
            }
            else
            if (Madera >= 1500)
            {
                m = 150;
            }
            else
            if (Madera >= 1000)
            {
                m = 100;
            }
            else if (Madera >= 500)
            {
                m = 50;
            }
            else if (Madera >= 250)
            {
                m = 25;
            }
            else if (Madera >= 150)
            {
                m = 15;
            }
            else if (Madera >= 100)
            {
                m = 10;
            }
            else if (Madera >= 50)
            {
                m = 5;
            }
            else if (Madera >= 25)
            {
                m = 3;
            }
            else if (Madera >= 10)
            {
                m = 1;
            }
            if (m > 0)
            {
                Madera -= m/CriarLvl;
                Poblacion += m;
                Historial.Text = "Madera -" + m/CriarLvl + "\nPoblacion +" + m + "\n" + Historial.Text;
                Comentario("Nuestro pueblo a adquirido guerreros, "+m+" para ser exacto");
            }
            else
            {
                Comentario("No tenemos madera, que esperas para atacar");
                Historial.Text = "No tenemos Madera\n" + Historial.Text;
            }
        }
    }
    public void _on_pausa_pressed()
    {
        God.yaco.estado = God.yaco.estado == EstadoGame.Jugando1 ? EstadoGame.Pausa : (God.yaco.estado == EstadoGame.Pausa ? EstadoGame.Jugando1 : God.yaco.estado);
    }
    public void _on_rendir_pressed()
    {
        MainMenu.Visible = true;
        GameMenu.Visible = false;
        Madera = _Madera; Arboles = _Arboles; Poblacion = _Poblacion;
        Comentario("Mis guerreros, mi pueblo, adios");
    }
    public void _on_expandRec_pressed()
    {
        SizeRec = 220;
        GameMenu.GetNode<Button>("Expand").Visible = false;
    }
    public void _on_compacRec_pressed()
    {
        SizeRec = 0;
        GameMenu.GetNode<Button>("Expand").Visible=true;
    }
    public void _on_play_pressed()
    {
        _1 = 0;
        _2 = 0;
        God.yaco.estado = EstadoGame.Jugando2;
        MainMenu.Visible = false;
        GameMenu.Visible = true;
        fTime = r.RandiRange(2, 7);
        Historial.Text = "Turno de la Naturaleza\n";
        Rec.Size *= Vector2.Down;
    }
    public void PowerUp()
    {
        if (God.yaco.estado == EstadoGame.Jugando1 && acciones > 0)
        {
            acciones--;
            _1++;
            if (_1 > 4)
            {
                Historial.Text = "Humanos sin PowerUps\n" + Historial.Text;
                return;
            }
            switch (_1)
            {
                case 1:
                    Historial.Text = "Humanos PowerUp 1\nPoblacion +15%\n" + Historial.Text;
                    Poblacion *= 1.15f;
                    Comentario("Power up usado 1, mas 15% de Poblacion");
                    break;
                case 2:
                    Historial.Text = "Humanos PowerUp 2\nPoblacion +25%\n" + Historial.Text;
                    Poblacion *= 1.25f;
                    Comentario("Power up usado 2, mas 25% de Poblacion, adelante guerreros");
                    break;
                case 3:
                    Historial.Text = "Humanos PowerUp 3\nArboles -15%\n" + Historial.Text;
                    Arboles /= 1.15f;
                    Comentario("Power up usado 3, menos 15% de Arboles");
                    break;
                case 4:
                    Historial.Text = "Humanos PowerUp 4\nArboles -25%\n" + Historial.Text;
                    Arboles /= 1.25f;
                    Comentario("Ultimo Power up, los Arboles caen 25%");
                    break;
            }
        }
    }
}