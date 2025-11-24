using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Configuration;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Vernyomasnaplo
{
    internal class Program
    {

        static DateTime szul_datum;
        static bool fut = true;
        static string bejelentkezettFelhasznalo = "";
        static bool bejelentkezve = false;
        static bool kivalasztva = false;
        static string[] menupontok = { "Regisztrálás", "Bejelentkezés", "Adatok hozzáadása", "Adatok módosítása", "Adatok megjelenítése", "Adat törlése", "Beállítások", "Kilépés" };
        static Action[] fuggvenyek = { Regisztral, Bejelentkezes, AdatokHozzaadasa, Modosit, Megjelenit, Torol, Beallit, Kilep };
        static int aktualis_menu_pont = 0;
        static int menupontok_szama = menupontok.Length;

        static ConsoleColor[] szinek = {
            ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow,
            ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Gray, ConsoleColor.Black
        };

        static string[] szinek_neve = {
            "Zöld","Piros","Kék","Sárga","Cian","Magenta","Szürke","Fekete"
        };



        static int szinek_szama = szinek.Length;
        static int alapszin = 0;
        static int alaphatter = 7;

        // 🔹 Fájlok elérési útja a projekt gyökerében
        static readonly string adatokFile = "Adatok.txt";
        static readonly string felhasznalokFile = "Felhaszn.txt";

        static List<string> adatok = new List<string>();

        static void Main(string[] args)



        {

            try
            {


                // 🔹 Fájlok létrehozása, ha nem léteznek
                if (!File.Exists(adatokFile)) File.Create(adatokFile).Close();
                if (!File.Exists(felhasznalokFile)) File.Create(felhasznalokFile).Close();

                // 🔹 Adatok beolvasása
                adatok.Clear();
                foreach (var sor in File.ReadAllLines(adatokFile))
                {
                    if (!string.IsNullOrWhiteSpace(sor))
                        adatok.Add(sor);
                }
                Console.BackgroundColor = szinek[alaphatter];
                while (fut)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = szinek[alaphatter];

                    Console.Clear();

                    int startIndex = bejelentkezve ? 2 : 0;
                    int endIndex = bejelentkezve ? menupontok.Length : 2;


                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (aktualis_menu_pont == i)
                        {
                            Console.ForegroundColor = szinek[alapszin];
                            Console.WriteLine(menupontok[i]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine(menupontok[i]);
                        }
                    }


                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Enter:

                            if (aktualis_menu_pont >= startIndex && aktualis_menu_pont < endIndex)
                                fuggvenyek[aktualis_menu_pont]();


                            if (bejelentkezve && aktualis_menu_pont < 2)
                                aktualis_menu_pont = 2;
                            break;

                        case ConsoleKey.UpArrow:
                            if (aktualis_menu_pont > startIndex)
                                aktualis_menu_pont--;
                            else
                            {
                                aktualis_menu_pont = endIndex - 1;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (aktualis_menu_pont < endIndex - 1)
                                aktualis_menu_pont++;
                            else
                            {
                                aktualis_menu_pont = 0;
                            }
                            break;

                        default:
                            Console.Beep();
                            break;
                    }


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        // 🔸 Felhasználó létezés ellenőrzés
        static bool FelhasznaloLetezik(string nev)
        {
            try
            {
                foreach (var sor in File.ReadAllLines(felhasznalokFile))
                {
                    var adatok = sor.Split(';');
                    if (adatok.Length > 0 && adatok[0] == nev)
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return false;
            }

        }

        static void Regisztral()
        {

            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║      Regisztrálás      ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                Console.WriteLine("Regisztrálás:\n");
                Console.Write("Felhasználónév: ");
                string nev = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(nev))
                {
                    Console.WriteLine("A felhasználónév nem lehet üres!");
                    Console.ReadLine();
                    return;
                }

                if (FelhasznaloLetezik(nev))
                {
                    Console.WriteLine("Ez a felhasználónév már létezik!");
                    Console.ReadLine();
                    return;
                }
                Console.Write("Születési dátum: ");
                szul_datum = DateTime.Parse(Console.ReadLine());


                Console.Write("Jelszó: ");
                string jelszo = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(jelszo))
                {
                    Console.WriteLine("A jelszó nem lehet üres!");
                    Console.ReadLine();
                    return;
                }
                File.AppendAllText(felhasznalokFile, $"{nev};{jelszo};{szul_datum}{Environment.NewLine}");
                File.AppendAllText(adatokFile, $"{nev}({Environment.NewLine}");
                adatok.Add($"{nev}(");
                Console.WriteLine("Sikeres regisztráció!");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        static void Bejelentkezes()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║      Bejelentkezés     ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                Console.WriteLine("Bejelentkezés:\n");

                Console.Write("Felhasználónév: ");
                string nev = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(nev))
                {
                    Console.WriteLine("A felhasználónév nem lehet üres!");
                    Console.ReadLine();
                    return;
                }

                Console.Write("Jelszó: ");
                string jelszo = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(jelszo))
                {
                    Console.WriteLine("A jelszó nem lehet üres!");
                    Console.ReadLine();
                    return;
                }

                bool sikeres = false;
                foreach (var sor in File.ReadAllLines(felhasznalokFile))
                {
                    var adatok = sor.Split(';');
                    if (adatok.Length >= 2 && adatok[0] == nev && adatok[1] == jelszo)
                    {
                        szul_datum = Convert.ToDateTime(adatok[2]);
                        sikeres = true;
                        break;
                    }
                }

                if (sikeres)
                {
                    bejelentkezve = true;
                    bejelentkezettFelhasznalo = nev;
                    Console.WriteLine("Sikeres bejelentkezés!");
                }
                else
                {
                    Console.WriteLine("Hibás felhasználónév vagy jelszó!");
                }
                Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        static void AdatokHozzaadasa()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║   Adatok hozzáadása    ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                bool talalat = false;
                string nev = "";
                int index = 0;
                DateTime most = DateTime.Now;

                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        nev = adatok[i].Split('(')[0];
                        talalat = true;
                        index = i;
                        break;
                    }
                }
                if (talalat == true)
                {
                    Console.Write("Adja meg a szisztolést: ");
                    int szisztoles = int.Parse(Console.ReadLine());

                    Console.Write("Adja meg a diasztolést: ");
                    int diasztoles = int.Parse(Console.ReadLine());


                    Console.Write("Adja meg a pulzusát: ");
                    int pulzus = int.Parse(Console.ReadLine());


                    string keszadat = $"{szisztoles};{diasztoles};{pulzus};{most}";

                    if (adatok[index].Split('(')[1] != "")
                    {
                        adatok[index] += "|";
                        adatok[index] += keszadat;
                    }
                    else
                    {
                        adatok[index] += keszadat;
                    }
                }
                else
                {
                    Console.WriteLine("Az Adatok.txt be nem került bele a felhasználó neved!");
                    Console.ReadLine();
                }
                File.WriteAllText(adatokFile, "");
                for (int i = 0; i < adatok.Count; i++)
                {
                    File.AppendAllText(adatokFile, adatok[i] + Environment.NewLine);
                }
                Console.WriteLine("Adat hozzáadva.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }

        }

        static void Modosit()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║   Adatok módosítása    ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                bool talalat = false;
                int index = 0;
                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        talalat = true;
                        index = i;
                        break;
                    }
                }

                Console.WriteLine("Adatok megjelenítése:\n");

                int darab = adatok[index].Split('(')[1].Split('|').Count();
                int törles;
                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;

                if (talalat == true)
                {
                    Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]} ({eletkor} éves)\n");
                    if (adatok[index].Split('(')[1] != "")
                    {
                        for (int i = 0; i < darab; i++)
                        {
                            Console.Write($"A(z) {i + 1}. mérés eredménye {adatok[index].Split('(')[1].Split('|')[i].Split(';')[3]} szisztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]} diasztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]} pulzus: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[2]}\n");
                        }
                    }
                }

                Console.Write("\nAdja meg a módosítandó mérés sorszámát: ");
                törles = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a szisztolést: ");
                int szisztoles = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a diasztolést: ");
                int diasztoles = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a pulzusát: ");
                int pulzus = int.Parse(Console.ReadLine());

                var modositas = $"{szisztoles};{diasztoles};{pulzus};{adatok[index].Split('(')[1].Split('|')[törles - 1].Split(';')[3]}";

                var start = adatok[index].IndexOf(adatok[index].Split('(')[1].Split('|')[törles - 1]);
                var vege = adatok[index].Split('(')[1].Split('|')[törles - 1].Length;

                adatok[index] = adatok[index].Remove(start, vege);

                adatok[index] = adatok[index].Insert(start, modositas);

                File.WriteAllText(adatokFile, "");
                for (int i = 0; i < adatok.Count; i++)
                {
                    File.AppendAllText(adatokFile, adatok[i] + Environment.NewLine);
                }
                Console.WriteLine("Adat módosítva.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        static void Megjelenit()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║  Adatok megjelenítése  ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;
                bool talalat = false;
                int index = 0;
                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        talalat = true;
                        index = i;
                        break;
                    }
                }
                Console.WriteLine("Adatok megjelenítése:\n");

                if (talalat == true)
                {
                    Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]} ({eletkor} éves)\n");
                    if (adatok[index].Split('(')[1] != "")
                    {
                        int darab = adatok[index].Split('(')[1].Split('|').Count();
                        if (darab > 0)
                        {
                            // Legnagyobbak
                            int leg_szisz = int.Parse(adatok[index].Split('(')[1].Split('|')[0].Split(';')[0]);
                            int leg_diasz = int.Parse(adatok[index].Split('(')[1].Split('|')[0].Split(';')[1]);
                            int leg_pul = int.Parse(adatok[index].Split('(')[1].Split('|')[0].Split(';')[2]);

                            // Legkisebbek
                            int legk_szisz = leg_szisz;
                            int legk_diasz = leg_diasz;
                            int legk_pul = leg_pul;

                            for (int i = 0; i < darab; i++)
                            {
                                int curr_szisz = int.Parse(adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]);
                                int curr_diasz = int.Parse(adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]);
                                int curr_pul = int.Parse(adatok[index].Split('(')[1].Split('|')[i].Split(';')[2]);

                                // Legnagyobbak
                                if (leg_szisz < curr_szisz) leg_szisz = curr_szisz;
                                if (leg_diasz < curr_diasz) leg_diasz = curr_diasz;
                                if (leg_pul < curr_pul) leg_pul = curr_pul;

                                // Legkisebbek
                                if (legk_szisz > curr_szisz) legk_szisz = curr_szisz;
                                if (legk_diasz > curr_diasz) legk_diasz = curr_diasz;
                                if (legk_pul > curr_pul) legk_pul = curr_pul;

                                Console.Write($"A(z) {i + 1}. mérés eredménye:\nDátum: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[3]}\nAdatok: szisztolés: {curr_szisz}, diasztolés: {curr_diasz}, pulzus: {curr_pul}\n" +
                                    $"Diagnózis: {Vernyomas(curr_szisz, curr_diasz)}  {pulzusfigyelo(curr_pul)}\n\n");
                            }

                            Console.WriteLine();
                            Console.WriteLine($"Legnagyobb szisztolés: {leg_szisz}");
                            Console.WriteLine($"Legnagyobb diasztolés: {leg_diasz}");
                            Console.WriteLine($"Legnagyobb pulzus: {leg_pul}");
                            Console.WriteLine($"Legkisebb szisztolés: {legk_szisz}");
                            Console.WriteLine($"Legkisebb diasztolés: {legk_diasz}");
                            Console.WriteLine($"Legkisebb pulzus: {legk_pul}");
                        }


                    }
                }

                







                Console.WriteLine("\nNyomjon Entert a visszatéréshez.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        static string Vernyomas(int szisztoles, int diasztoles)
        {
            try
            {
                if (szisztoles <= 90 && diasztoles <= 60) { return "Figyelem: a vérnyomás értéke alacsony."; }
                else if (szisztoles <= 120 && diasztoles <= 80) { return "A vérnyomása normális."; }
                else if (szisztoles <= 140 && diasztoles <= 90) { return "Figyelem: a vérnyomása emelkedett."; }
                else if (szisztoles <= 160 && diasztoles <= 100) { return "Figyelem: a vérnyomás értéke magas! Ez 1. fokú hipertónia."; }
                else if (szisztoles <= 180 && diasztoles <= 110) { return "Figyelem: a vérnyomás értéke magas! Ez 2. fokú hipertónia."; }
                else { return "Figyelem: a vérnyomás értéke magas! Ez 3. fokú hipertónia."; }
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return "hiba a Vernyomásnál";
            }
        }

        static void Torol()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║     Adatok törlése     ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");


                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;
                bool talalat = false;
                int index = 0;
                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        talalat = true;
                        index = i;
                        break;
                    }
                }
                Console.WriteLine("Adatok megjelenítése:\n");
                int darab = 0;
                if (talalat == true)
                {
                    darab = adatok[index].Split('(')[1].Split('|').Count();
                    Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]} ({eletkor} éves)\n");
                    if (adatok[index].Split('(')[1] != "")
                    {
                        for (int i = 0; i < darab; i++)
                        {

                            Console.Write($"A(z) {i + 1}. mérés eredménye:\nDátum: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[3]}\nAdatok: szisztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]}, diasztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]}, pulzus: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[2]}\n");
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("Adja meg melyiket kívánja törölni?");

                    Console.WriteLine(adatok[index]);
                    int torol_index = int.Parse(Console.ReadLine()) - 1;





                    string torolni = $"{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[0]};{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[1]};{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[2]};{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[3]}";

                    if (torol_index > 0 && darab > 1)
                    {
                        torolni = "|" + torolni;

                    }
                    else if (darab > 1)
                    {
                        torolni += "|";
                    }
                    Console.WriteLine(torolni);
                    string modositott = adatok[index].Replace(torolni, "");
                    Console.WriteLine(modositott);

                    string[] sorok = File.ReadAllLines("Adatok.txt");
                    sorok[index] = modositott;

                    File.WriteAllLines("Adatok.txt", sorok);
                    adatok[index] = modositott;


                }





                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }



        static string pulzusfigyelo(int pulzus)
        {
            try
            {

                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;

                if (eletkor >= 0 && eletkor <= 1) // Csecsemő
                {
                    if (pulzus < 120) return "Alacsony a pulzusod.";
                    else if (pulzus > 160) return "Magas a pulzusod.";
                }
                else if (eletkor > 1 && eletkor <= 2) // Kisgyermek
                {
                    if (pulzus < 110) return "Alacsony a pulzusod.";
                    else if (pulzus > 150) return "Magas a pulzusod.";
                }
                else if (eletkor > 2 && eletkor <= 5) // Óvodás
                {
                    if (pulzus < 80) return "Alacsony a pulzusod.";
                    else if (pulzus > 120) return "Magas a pulzusod.";
                }
                else if (eletkor > 5 && eletkor <= 12) // Iskolás
                {
                    if (pulzus < 70) return "Alacsony a pulzusod.";
                    else if (pulzus > 110) return "Magas a pulzusod.";
                }
                else if (eletkor > 12 && eletkor <= 18) // Serdülő
                {
                    if (pulzus < 60) return "Alacsony a pulzusod.";
                    else if (pulzus > 100) return "Magas a pulzusod.";
                }
                else if (eletkor >= 19) // Felnőttek (19 év felett)
                {
                    if (pulzus < 60) return "Alacsony a pulzusod.";
                    else if (pulzus > 100) return "Magas a pulzusod.";
                }

                return "Normális a pulzusod.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return "hiba a pulzus figyeloben";
            }
        }

        static void Beallit()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                string[] kiirni = { "Betű", "Háttér" };
                bool megy = true;
                int aktualis = 0;
                while (megy)
                {
                    Console.Clear();

                    for (int i = 0; i < kiirni.Length; i++)
                    {
                        if (aktualis == i)
                        {
                            Console.ForegroundColor = szinek[alapszin];
                            Console.WriteLine(kiirni[i]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine(kiirni[i]);
                        }
                    }

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Enter:
                            int szin = Szinvalaszto();
                            if (szin == -1)
                            {
                                break;
                            }
                            if (alaphatter != szin && alapszin != szin)
                            {
                                megy = false;
                                if (aktualis == 0)
                                    alapszin = szin;
                                else
                                    alaphatter = szin;
                            }

                            break;

                        case ConsoleKey.DownArrow:
                            if (aktualis < kiirni.Length - 1)
                                aktualis++;
                            else
                            {
                                aktualis = 0;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (aktualis > 0)
                                aktualis--;
                            else
                            {
                                aktualis = kiirni.Length - 1;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            megy = false;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        static int Szinvalaszto()
        {
            try
            {


                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];


                ConsoleColor[] szinek_kicsi = new ConsoleColor[szinek.Length - 1];
                string[] szinek_neve_kicsi = new string[szinek_neve.Length - 1];
                int jo_index = 0;
                for (int i = 0; i < szinek.Length; i++)
                {
                    if (i == alapszin || i == alaphatter)
                        continue; // ezt a színt kihagyjuk

                    szinek_kicsi[jo_index] = szinek[i];
                    szinek_neve_kicsi[jo_index] = szinek_neve[i];
                    jo_index++;
                }


                bool kivalaszt = true;
                int akt_szin_szama = 0;

                while (kivalaszt)
                {
                    Console.Clear();

                    for (int i = 0; i < szinek_kicsi.Length; i++)
                    {



                        if (i == akt_szin_szama)
                        {

                            Console.ForegroundColor = szinek_kicsi[i];
                            Console.WriteLine(szinek_neve_kicsi[i]);
                            Console.ForegroundColor = ConsoleColor.White;


                        }
                        else
                        {
                            Console.WriteLine(szinek_neve_kicsi[i]);
                        }

                    }

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Enter:
                            kivalaszt = false;
                            break;
                        case ConsoleKey.UpArrow:

                            if (akt_szin_szama > 0)
                                akt_szin_szama--;
                            else
                            {
                                akt_szin_szama = szinek_szama - 3;
                            }
                            break;
                        case ConsoleKey.DownArrow:

                            if (akt_szin_szama < szinek_szama - 3)
                                akt_szin_szama++;
                            else
                            {
                                akt_szin_szama = 0;
                            }
                            break;
                        default:
                            Console.Beep();
                            break;
                        case ConsoleKey.LeftArrow:
                            kivalaszt = false;
                            return -1;

                    }
                }

                int vissza = Array.IndexOf(szinek, szinek_kicsi[akt_szin_szama]);
                return vissza;
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return 0;
            }
        }

        static void Kilep()
        {
            fut = false;
        }
    }
}