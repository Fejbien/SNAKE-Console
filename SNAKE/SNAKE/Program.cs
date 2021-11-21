using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csZadanie
{
    class Program
    {
        static int ODSTEPCZASOWY = (int)Trudnosci.Normalny;     // Odstep w milisekundach kazdej "klatki" domyslnie normalny
        static readonly int WIELKOSC = 20;                      // Wliczajac sciany czyli grywalna planasz przy ustawionych 20 to 18 x 18 

        static readonly char ZNAKWEZA = '#';                    // Latwy do zmiany znak weza
        static readonly char ZNAKJABLKA = (char)6;              // Latwy do zmiany znak jablka 
        // Znak jablka to znak pik z kart moze sie roznic w zaleznosci od kodowania na danym urzadzeniu
        // Do znalezienia znaku uzylem funkcji ktora pokazuje znak przypisany do danej liczby (funkcje zostawilem jest na dole kodu jakby cos)

        enum Kierunki
        {
            GORA,
            DOL,
            LEWO,
            PRAWO
        }

        enum Trudnosci
        {
            // Latwo do zmienienia odstepy czasowe miedzy klatekami w milisekunach w zaleznosci od poziomu trudnosci
            Latwy = 350,
            Normalny = 200,
            Trudny = 100
        }

        // Zwykla klasa z koordami x i y nazwana tak z konfencji z UNITY
        class Vector2D
        {
            public int x;
            public int y;

            public Vector2D(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }

        static void Main(string[] args)
        {
            Random rnd = new Random();

            // Menu wyboru trudnosc
            Menu();

            // Pentla calej gry z resetowaniem
            while (true)
            {
                // Tworzy liste z cialem weza i dodaje pierwszy element
                LinkedList<Vector2D> cialoWeza = new LinkedList<Vector2D>();
                cialoWeza.AddFirst(new Vector2D(WIELKOSC / 2, WIELKOSC / 2));

                // Tworzy jablko w losowym miejscu ustawia punkty na 1 i kierunek w ktorym jedzie waz
                Vector2D jablko = WylosujJablko(cialoWeza, rnd);
                int punkty = 1;
                Kierunki kierunek = Kierunki.GORA;

                // Rysuje plansze i jablko
                NarysujPlansze();
                DorysujJablko(jablko);

                // Pentla gry weza
                while (true)
                {
                    kierunek = SprawdzenieZmianyKierunku(kierunek); // Zmienia kierunek poruszania sie weza
                    RuchSprawdzanieJablkaIRysowanie(cialoWeza, kierunek, ref jablko, rnd, ref punkty); // Zajmuje sie poruszaniem weza, jedzeniem jablka i rysowaniem ich

                    // Sprawdza koniec gry przez smierc weza
                    if (Smierc(cialoWeza))
                        break;

                    // Odstep czasowy jakby takie klatki
                    Thread.Sleep(ODSTEPCZASOWY);
                }

                // Ustawia kursor na dol konsoli i pyta sie czy konczyc gre
                Console.SetCursorPosition(0, WIELKOSC);
                Console.WriteLine("Aby grac dalej napisz 1 inne opcje koncza program!");
                if (int.TryParse(Console.ReadLine(), out int numer))
                {
                    if (numer != 1)
                        Environment.Exit(0);
                }
                else
                    Environment.Exit(0);
            }
        }

        static void Menu()
        {
            // Pokazuje menu
            Console.WriteLine("MENU");
            Console.WriteLine("Wybierz poziom trudnosci: ");
            Console.WriteLine("1. Latwy");
            Console.WriteLine("2. Normalny");
            Console.WriteLine("3. Trudny");
            Console.WriteLine("Kazde inne opcje koncza program!");

            // Wybor trudnosci
            int wybor;
            do { } while (!int.TryParse(Console.ReadLine(), out wybor));

            switch (wybor)
            {
                case 1:
                    ODSTEPCZASOWY = (int)Trudnosci.Latwy;
                    break;
                case 2:
                    ODSTEPCZASOWY = (int)Trudnosci.Latwy;
                    break;
                case 3:
                    ODSTEPCZASOWY = (int)Trudnosci.Trudny;
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        }

        static void RuchSprawdzanieJablkaIRysowanie(LinkedList<Vector2D> _cialoWeza, Kierunki kierunek, ref Vector2D _jablko, Random _rnd, ref int _punkty)
        {
            // Rusza weza w ustawiony kierunek
            switch (kierunek)
            {
                case Kierunki.GORA:
                    _cialoWeza.AddFirst(new Vector2D(_cialoWeza.First.Value.x, _cialoWeza.First.Value.y - 1));
                    break;
                case Kierunki.DOL:
                    _cialoWeza.AddFirst(new Vector2D(_cialoWeza.First.Value.x, _cialoWeza.First.Value.y + 1));
                    break;
                case Kierunki.LEWO:
                    _cialoWeza.AddFirst(new Vector2D(_cialoWeza.First.Value.x - 1, _cialoWeza.First.Value.y));
                    break;
                case Kierunki.PRAWO:
                    _cialoWeza.AddFirst(new Vector2D(_cialoWeza.First.Value.x + 1, _cialoWeza.First.Value.y));
                    break;
            }

            // Usuwa calego weza
            ZmarzWeza(_cialoWeza);

            // Sprawdza czy zostalo zjedzone jablko
            if (_cialoWeza.First.Value.x == _jablko.x && _cialoWeza.First.Value.y == _jablko.y)
            {
                _jablko = WylosujJablko(_cialoWeza, _rnd);
                _punkty++;
                DorysujJablko(_jablko);
            }
            else
            {
                _cialoWeza.RemoveLast();
            }

            // Rysuje calego weza
            NarysujWeza(_cialoWeza);

            // Ustawia kursor na prawa strona od planszy i pisze dane rzeczy
            Console.SetCursorPosition(WIELKOSC, 1);
            Console.WriteLine($"Wyglad weza: {ZNAKWEZA}");
            Console.SetCursorPosition(WIELKOSC, 2);
            Console.WriteLine($"Wyglad jablka: {ZNAKJABLKA}");
            Console.SetCursorPosition(WIELKOSC, 3);
            Console.WriteLine($"Punkty: {_punkty}");

            // Przesuwa znak pisania zeby na planysz nie bylo takiego miejsca zaznaczenego gdzie pisac
            Console.SetCursorPosition(WIELKOSC, WIELKOSC);
        }

        private static void ZmarzWeza(LinkedList<Vector2D> _cialoWeza)
        {
            // Usuwa calego weza
            foreach (var item in _cialoWeza)
            {
                Console.SetCursorPosition(item.x, item.y);
                Console.Write(" ");
            }
        }

        static void NarysujWeza(LinkedList<Vector2D> _cialoWeza)
        {
            // Rysuje calego weza
            foreach (var item in _cialoWeza)
            {
                Console.SetCursorPosition(item.x, item.y);
                Console.Write(ZNAKWEZA);
            }
        }

        static Vector2D WylosujJablko(LinkedList<Vector2D> _cialoWeza, Random _rnd)
        {
            // Losouje jablko
            Vector2D temp;
            do
            {
                temp = new Vector2D(_rnd.Next(1, WIELKOSC - 1), _rnd.Next(1, WIELKOSC - 1));
            } while (Sprawdzenie());

            return temp;

            // Spradza czy miejsce wylosowanego jablka juz jest waz
            bool Sprawdzenie()
            {
                foreach (var item in _cialoWeza)
                {
                    if (item.x == temp.x && item.y == temp.y)
                        return true;
                }
                return false;
            }
        }

        static bool Smierc(LinkedList<Vector2D> _cialoWeza)
        {
            // Sprawdzenie wyjechania
            if (_cialoWeza.First.Value.x >= WIELKOSC - 1 || _cialoWeza.First.Value.x <= 0)
                return true;

            if (_cialoWeza.First.Value.y >= WIELKOSC - 1 || _cialoWeza.First.Value.y <= 0)
                return true;

            // Sprawdzenie czy nie wjechal w siebie
            for (int i = 1; i < _cialoWeza.Count; i++)
            {
                if (_cialoWeza.First.Value.x == _cialoWeza.ElementAt(i).x && _cialoWeza.First.Value.y == _cialoWeza.ElementAt(i).y)
                    return true;
            }

            return false;
        }

        static Kierunki SprawdzenieZmianyKierunku(Kierunki _kierunek)
        {
            // Ustawia nowy kierunek tylko wtedy kiedy przycisk bedzie wcisniety
            if (Console.KeyAvailable)
            {
                var klawisz = Console.ReadKey(true);
                switch (klawisz.KeyChar)
                {
                    case 'w':
                        if (_kierunek != Kierunki.DOL)
                            _kierunek = Kierunki.GORA;
                        break;
                    case 's':
                        if (_kierunek != Kierunki.GORA)
                            _kierunek = Kierunki.DOL;
                        break;
                    case 'a':
                        if (_kierunek != Kierunki.PRAWO)
                            _kierunek = Kierunki.LEWO;
                        break;
                    case 'd':
                        if (_kierunek != Kierunki.LEWO)
                            _kierunek = Kierunki.PRAWO;
                        break;
                }
            }

            return _kierunek;
        }

        static void DorysujJablko(Vector2D _jablko)
        {
            // Rysuje jablko
            Console.SetCursorPosition(_jablko.x, _jablko.y);
            Console.Write(ZNAKJABLKA);
        }

        static void NarysujPlansze()
        {
            // Resetuje i rysuje pelna plansze
            Console.Clear();

            // Gora i doł
            for (int i = 0; i < WIELKOSC; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("-");

                Console.SetCursorPosition(i, WIELKOSC - 1);
                Console.Write("-");
            }

            // Lewo prawo
            for (int i = 0; i < WIELKOSC; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("|");

                Console.SetCursorPosition(WIELKOSC - 1, i);
                Console.Write("|");
            }

            // Ustawia ladniejsze rogi
            Console.SetCursorPosition(0, 0);
            Console.Write("+");
            Console.SetCursorPosition(0, WIELKOSC - 1);
            Console.Write("+");
            Console.SetCursorPosition(WIELKOSC - 1, 0);
            Console.Write("+");
            Console.SetCursorPosition(WIELKOSC - 1, WIELKOSC - 1);
            Console.Write("+");
        }

        static void ListaZnakow()
        {
            Console.Clear();
            for (int i = 0; i < 255; i++)
                Console.WriteLine($"{i}. {(char)i}");
        }
    }
}