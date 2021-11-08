using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using System.Net;
using System.Timers;

namespace DontDiePart2
{
    /* Class: Question
     * Author: Jonathan Karcher
     * Purpose: Store a list of questions from the web
     */
    class Question
    {
        public int responceCode;
        public List<QuestionResults> results;
    }
    /* Class: QuestionResults
     * Author: Jonathan Karcher
     * Purpose: Store the Json data of a Question
     */
    class QuestionResults
    {
        public string category;
        public string type;
        public string dificulty;
        public string question;
        public string correct_answer;
        public List<string> incorrect_answers;
    }
    /* Class: _
     * Author: Jonathan Karcher
     * Purpose: Used for readability assistance and localize all custom static variables
     */
    public static class _
    {
        public static int A = 0;
        public static int B = 1;
        public static int C = 2;
        public static int D = 3;
        public static int E = 4;
        public static int F = 5;
        public static int G = 6;
        public static int H = 7;
        public static bool gameOver = false;
        public static int start = _.A;
        public static int goal = _.H;
        public static Random random = new Random();
        public static bool timeIsUp = false;
        public static Timer timer = new Timer();
        public static int playersOldPosition = _.A;
    }
    /* Class: Direction
     * Author: Jonathan Karcher
     * Purpose: Contain the specific data related to traveling in one direction
     */
    class Direction
    {
        // the room that can be traveled to
        private int destination;
        // the cost of traveling to the destination
        private int weight;
        /* Constructor: Direction
         * Purpose: Set the initial directional data
         * Restrictions: None
         */
        public Direction(int destination, int weight)
        {
            this.destination = destination;
            this.weight = weight;
        }
        /* Property: Destination
         * purpose: Return the value of destination
         * Restrictions: Read only
         */
        public int Destination { get { return destination; } }
        /* Property: Weight
         * purpose: Return the value of weight
         * Restrictions: Read only
         */
        public int Weight { get { return weight; } }
    }
    /* Class: Room
     * Author: Jonathan Karcher
     * Purpose: Manages the 4 potential directions that the player can travel
     */
    class Room
    {
        // the description of the room
        private string description;
        // the four cardinal directions
        private Direction north;
        private Direction south;
        private Direction east;
        private Direction west;
        /* Constructor: Room
         * Purpose: Set the initial Room data
         * Restrictions: None
         */
        public Room(Direction north, Direction south, Direction east, Direction west, string description)
        {
            this.north = north;
            this.south = south;
            this.east = east;
            this.west = west;
            this.description = description;
            //north = null;
            //south = null;
            //east = null;
            //west = null;
        }
        /* Property: North
         * purpose: Return the value of north
         * Restrictions: Read only
         */
        public Direction North { get { return north; } }
        /* Property: South
         * purpose: Return the value of south
         * Restrictions: Read only
         */
        public Direction South { get { return south; } }
        /* Property: East
         * purpose: Return the value of east
         * Restrictions: Read only
         */
        public Direction East { get { return east; } }
        /* Property: West
         * purpose: Return the value of west
         * Restrictions: Read only
         */
        public Direction West { get { return west; } }
        /* Property: Description
         * purpose: Return the value of description
         * Restrictions: Read only
         */
        public string Description { get { return description; } }
    }
    /* Class: Story
     * Author: Jonathan Karcher
     * Purpose: Contains and organizes the story text elements 
     */
    class Story
    {
        private string howToPlay;
        private string greeting;
        private string death;
        private string[] askAQuestion = new string[4];
        private string[] answerWasRight = new string[4];
        private string[] answerWasWrong = new string[4];
        private string[] ranOutOfTime = new string[4];
        private string[] randomDamage = new string[4];
        private string[] walkDamage = new string[4];
        private string[] roomDescription = new string[8];
        /* Constructor: Story
         * Purpose: Assign the stroy text elements
         * Restrictions: None
         */
        public Story()
        {
            // instructions on how to play
            howToPlay = "How to play: As you play the game you will see the prompts (Gamble) (North) (South) (East) (West).\n" +
                "Entering any of these values case insensitive, when the prompt is available will allow the game to proceed.\n" +
                "The goal of the game is to escape the labyrinth.\n" +
                "Your Health: is the amount of health you have left.\n" +
                "(Gamble) allows the player to gamble any amount of health up to the value displayed in \"Your Health:\".\n" +
                "You can only die while gambling your life. You die when the value reaches 0.\n" +
                "Wagering your life requires an integer value up to the value of health that the player has.\n" +
                "You will have 15 seconds to answer the question.\n" +
                "(North) Travel north, (South) travel south, (East) travel east, (West) travel west.\n" +
                "Traveling down any path will result in loss of health.\n" +
                "Access to a room is only available if you have enough health to travel to it.\n";

            // initial greeting
            greeting = "A shrill high pitch voice announces, \"Ahhh... it wakes, I was beginning to get bored!\"\n" +
                "You call out, \"Who’s there?\"\n" +
                "\"A game of riddles is what I sought, and should you fail your life be naught!\"\n" +
                "... clearly whoever is talking to you doesn’t want to answer your question, \nbut they aren’t attacking you... yet, either.\n" +
                "You ask the darkness \"What are the rules?\"\n" +
                "\"The question, the game, the words to maim will flow forth from me to you, \nand to the dark one phrase you need, this I promise you.\"\n" +
                "... ok so it seems that there will be a question that I will need to answer, \"the words to maim\" \nseems to mean that there are consequences if I’m wrong.\n";

            // if you die
            death = "In an instant your body fails to respond to you, but time doesn’t seem to stop or even slow.\n" +
                "You lay on the floor for what seems like hours... maybe days, confused you can only wonder \"what happens after\".\n";

            // when asking a question
            askAQuestion[0] = "Ok voice in the dark I’m ready, ask your question.\n";
            askAQuestion[1] = "Are you there? I’m ready for a question.\n";
            askAQuestion[2] = "Hey creepy crawler I don’t have all day, make with the question.\n";
            askAQuestion[3] = "A shrill voice in the dark \"You have been quiet, are you ready to play?\" Yes, ask your question.\n";

            // if the answer was right
            answerWasRight[0] = "You feel imbued with new life as the voice cackles with joy \"Your right!\"... that’s strange does it actually \nwant me to win.\n";
            answerWasRight[1] = "A wound you have been trying to ignore begins to rapidly heal as you hear the voice say \"Your right!\"\n";
            answerWasRight[2] = "A warm sensation envelopes you, the voice with a gleeful tone says \"Your right!\"\n";
            answerWasRight[3] = "You are filled with energy as the whole room seems to become brighter. The voice announces \"Your right!\"\n";

            // if the answer was wrong
            answerWasWrong[0] = "You drop to your knee as you feel the energy leave your body, as you struggle to stay conscious you hear the word \n\"Wrong.\"\n";
            answerWasWrong[1] = "A fit of coughing takes over as you feel all the air being forced from your lungs. The voice in its confusingly \ncheerful tone announces \"Wrong.\"\n";
            answerWasWrong[2] = "In a flash a cut appears on your chest, you clutch the wound as you look for the source as the voice announces \"Wrong.\"\n";
            answerWasWrong[3] = "For a moment your vision fades and in a panic you stumble and fall, hitting your head against the floor. \nThe voice announces \"Wrong.\"\n";

            // if the player runs out of time
            ranOutOfTime[0] = "A deep chuckle fills the room, different from the voice.  With a loud boom you hear \"Times Up.\"\n";
            ranOutOfTime[1] = "I stumped you this time didn’t I... either way \"Times Up.\"\n";
            ranOutOfTime[2] = "Well well well it seems \"Times Up.\", if you keep this up you'll be in the bone yard before you know it.\n";
            ranOutOfTime[3] = "I’m growing bored with your silence... it’s not wise to BORE me. \"Times Up.\"\n";

            // when walking from room to room
            walkDamage[0] = "The path seems to have been longer than you expected and your legs ach as you come to the end of the path.\n";
            walkDamage[1] = "As you walk through the dark you trip over a rock and fall to the ground.\n";
            walkDamage[2] = "In the dark you misstep and trip over your own feet.\n";
            walkDamage[3] = "A rock falls from the stone ceiling striking you on the head.\n";

            // when walking from room to room
            randomDamage[0] = "Leaping from the dark something small and furry claws at your ankles and flees off into the dark.\n";
            randomDamage[1] = "A rock falls from the stone ceiling striking you on the head.\n";
            randomDamage[2] = "In the dark you misstep and trip over your own feet.\n";
            randomDamage[3] = "As you walk through the dark you trip over a rock and fall to the ground.\n";

            // the descriptions of the room
            roomDescription[_.A] = "As you look around, the room is dark with barley enough light to see.  \nThe walls are slick though it’s too dark to tell with what, the floor seems \nto be of flattened stone and the ceiling... if there is one, is as dark as the void. \n";
            roomDescription[_.B] = "You emerge in a dimly lit room with hardwood floors in the center a table with a candelabra \nin the shape of a man burns... you probably shouldn’t touch it.  \nThe walls are painted in a substance that shimmers like gold in the candlelight.  \n";
            roomDescription[_.C] = "The room seems to be a perfect dome with a mirror finish, at its center a fountain flows with a strange purple fluid. \n";
            roomDescription[_.D] = "As you enter the room you see bones scattered across the floor human and animal alike, it seems something was... \nor, is here and it’s not friendly \n";
            roomDescription[_.E] = "You’re in a room... at least your pretty sure you are.  \nIt’s so dark you can’t see anything except a faint light a path perhaps. \n";
            roomDescription[_.F] = "The walls are the color of blood and slick with something clear. \nThe floor seems to squish with every step... as you look closer you can see veins pulsing throughout the room.  \n";
            roomDescription[_.G] = "It’s cold in this room, small puddles on the floor are frozen solid and snow falls from the blindingly white ceiling \nto create a sort of hourglass... you probably shouldn’t linger or you may become trapped.  \n";
            roomDescription[_.H] = "The light... for the first time you can see the sunshine.  \nYou have survived the labyrinth.\n";
        }
        /* Property: HowToPlay
         * purpose: Return the value of howToPlay
         * Restrictions: Read only
         */
        public string HowToPlay { get { return howToPlay; } }
        /* Property: Greeting
         * purpose: Return the value of greeting
         * Restrictions: Read only
         */
        public string Greeting { get { return greeting; } }
        /* Property: Death
         * purpose: Return the value of death
         * Restrictions: Read only
         */
        public string Death { get { return death; } }
        /* Method: RoomDescription
         * Purpose: Get a specific room description
         * Restrictions: None
         */
        public string RoomDescription(int room) { return roomDescription[room]; }
        /* Method: AskAQuestion
         * Purpose: Get a specific question responce
         * Restrictions: None
         */
        public string AskAQuestion(int val) { return askAQuestion[val]; }
        /* Method: AnswerWasRight
         * Purpose: Get a specific right answer responce
         * Restrictions: None
         */
        public string AnswerWasRight(int val) { return answerWasRight[val]; }
        /* Method: AnswerWasWrong
         * Purpose: Get a specific wrong answer responce
         * Restrictions: None
         */
        public string AnswerWasWrong(int val) { return answerWasWrong[val]; }
        /* Method: RanOutOfTime
         * Purpose: Get a specific wrong answer ranOutOfTime
         * Restrictions: None
         */
        public string RanOutOfTime(int val) { return ranOutOfTime[val]; }
        /* Method: WalkDamage
         * Purpose: Get a specific walk damage responce
         * Restrictions: None
         */
        public string WalkDamage(int val) { return walkDamage[val]; }
        /* Method: RandomDamage
         * Purpose: Get a specific random damage responce
         * Restrictions: None
         */
        public string RandomDamage(int val) { return randomDamage[val]; }
    }
    /* Class: Player
     * Author: Jonathan Karcher
     * Purpose: Manages data Related to player movement
     */
    class Player
    {
        private int position;
        private int health;
        private Story story;
        /* Constructor: Player
         * Purpose: Set the initial player position in the maze
         * Restrictions: None
         */
        public Player(int startPos, Story story)
        {
            position = startPos;
            health = 1;
            this.story = story;
        }
        /* Property: Position
         * purpose: Return the value of position
         * Restrictions: Read only
         */
        public int Position { get { return position; } }
        /* Property: Health
         * purpose: Return the value of health
         * Restrictions: Read only
         */
        public int Health { get { return health; } }
        /* Method: ResetPlayer
         * Purpose: Reset the game data to a "new game" state
         * Restrictions: None
         */
        public void ResetPlayer()
        {
            Console.WriteLine(story.Greeting);
            _.gameOver = false;
            position = _.start;
            health = 1;
        }
        public void Move((string, int)[,] matrixIn, int goal)
        {
            // assemble a room from the matrix
            Direction localNorth = null;
            Direction localSouth = null;
            Direction localEast = null;
            Direction localWest = null;
            for (int i = 0; i < matrixIn.GetLength(1); i++)
            {
                if (matrixIn[Position, i].Item1 != null)
                {
                    if (matrixIn[Position, i].Item1.Contains("N"))
                    {
                        localNorth = new Direction(i, matrixIn[Position, i].Item2);
                    }
                }
            }
            for (int i = 0; i < matrixIn.GetLength(1); i++)
            {
                if (matrixIn[Position, i].Item1 != null)
                {
                    if (matrixIn[Position, i].Item1.Contains("E"))
                    {
                        localEast = new Direction(i, matrixIn[Position, i].Item2);
                    }
                }
            }
            for (int i = 0; i < matrixIn.GetLength(1); i++)
            {
                if (matrixIn[Position, i].Item1 != null)
                {
                    if (matrixIn[Position, i].Item1.Contains("S"))
                    {
                        localSouth = new Direction(i, matrixIn[Position, i].Item2);
                    }
                }
            }
            for (int i = 0; i < matrixIn.GetLength(1); i++)
            {
                if (matrixIn[Position, i].Item1 != null)
                {
                    if (matrixIn[Position, i].Item1.Contains("W"))
                    {
                        localWest = new Direction(i, matrixIn[Position, i].Item2);
                    }
                }
            }
            Room room = new Room(localNorth, localSouth, localEast, localWest, story.RoomDescription(Position));
            // direction reference initialized to null
            Direction dir = null;
            // player input initialized to empty
            string input = "";
            // the amount of health that the player will wager
            int amount;
            // display any valid path
            Console.Write(AvailableMoves(room));
            // read the players input
            input = Console.ReadLine().ToUpper();
            // Note: the string can be anything just not empty or there is an error
            if (input.Equals(""))
            {
                input = "_";
            }
            Console.WriteLine();
            // if the player wants to Gamble
            if (input[0] == 'G')
            {
                //amount = 0;
                // output a story element
                Console.WriteLine(story.AskAQuestion(_.random.Next(4)));
                Console.WriteLine("Exellent, How much are you willing to wager?\n");
                // get an acceptable integer value for the amount of health the player wants to risk
                do
                {
                    input = Console.ReadLine();
                    Console.WriteLine();
                    if (int.TryParse(input, out amount))
                    {
                        // if they try to barter health that they dont have
                        if (amount > health)
                        {
                            Console.WriteLine("You dont have that much life to wager.\n");
                        }
                        // if they try to cheat the system by entering a negative number and intentionally getting questions wrong
                        if (amount < 0)
                        {
                            Console.WriteLine("HA, nice try but no, entering a negative value wont get you out of this one.\n");
                            // I think ill still alow a value of 0 so the player can try a few questions before risking death
                        }
                    }
                    // if they didnt enter a number
                    else
                    {
                        Console.WriteLine("Thats not an integer.\n");
                    }
                } while (!int.TryParse(input, out amount) || amount > health || amount < 0);
                health += GambleForHealth(amount);
                // exit the function to avoid any drop through effects
                return;
            }
            // if the player does not want to gamble
            // chech if the player has enough life and entered a valid direction
            if (input[0] == 'N' && room.North != null && Health > room.North.Weight)
            {
                // if so set the direction to that
                dir = room.North;
            }
            else if (input[0] == 'S' && room.South != null && Health > room.South.Weight)
            {
                dir = room.South;
            }
            else if (input[0] == 'E' && room.East != null && Health > room.East.Weight)
            {
                dir = room.East;
            }
            else if (input[0] == 'W' && room.West != null && Health > room.West.Weight)
            {
                dir = room.West;
            }
            // if they entered a valid direction
            if (dir != null)
            {
                if (Health > dir.Weight)
                {
                    TakeWalkDamage(dir);
                }
            }
            // if they enterd an invalid direction
            else
            {
                if (input[0] == 'N' || input[0] == 'S' || input[0] == 'E' || input[0] == 'W')
                {
                    Console.WriteLine("That is not a direction you can go.\n");
                }
                else
                {
                    Console.WriteLine("That is not a direction.\n");
                }
            }
        }
        /* Method: AvailableMoves
         * Purpose: Assemble a string based on the avaialable actions
         * Restrictions: None
         */
        public string AvailableMoves(Room room)
        {
            // gambling is always an option
            string toReturn = "(Gamble) ";
            // build the display of player oprions
            if (room.North != null && Health > room.North.Weight)
            {
                toReturn = toReturn + "(North) ";
                Console.WriteLine("You can see a path to the north.");
            }
            if (room.South != null && Health > room.South.Weight)
            {
                toReturn = toReturn + "(South) ";
                Console.WriteLine("You can see a path to the south.");
            }
            if (room.East != null && Health > room.East.Weight)
            {
                toReturn = toReturn + "(East) ";
                Console.WriteLine("You can see a path to the east.");
            }
            if (room.West != null && Health > room.West.Weight)
            {
                toReturn = toReturn + "(West) ";
                Console.WriteLine("You can see a path to the west.");
            }
            // tell teh player their health status
            Console.WriteLine("\nYour Health: " + Health);
            toReturn = toReturn + ": ";
            return toReturn;
        }
        /* Method: GambleForHealth
         * Purpose: Ask the player a question and depending on whether or not they are right add or remove health from the player
         * Restrictions: None
         */
        public int GambleForHealth(int amount)
        {
            int toReturn = 0;
            string playerAnswer = "";
            // question reference
            Question question;
            // a list of answers
            LinkedList<string> answers = new LinkedList<string>();
            // a queue for the mixed up list of answers
            Queue<string> answersMixed = new Queue<string>();
            // a string representing the value stored in an answers node
            string node;
            // an int for the number of elements stored in the answers list
            int answerCout;
            // a reference for a random index to mix up the answers
            int randomIndex;
            // a reference to display the input options for answering the questions
            int index;
            // a reference used to find the right answer
            int indexToCheck;
            // url where questions originate
            string url = "https://opentdb.com/api.php?amount=1&category=15&type=multiple";
            // streams to manage getting the question and answers from the web
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            // read the question and answers from the web
            string s = reader.ReadToEnd();
            reader.Close();
            // make the string human readable
            question = JsonConvert.DeserializeObject<Question>(s);
            question.results[0].question = HttpUtility.HtmlDecode(question.results[0].question);
            question.results[0].correct_answer = HttpUtility.HtmlDecode(question.results[0].correct_answer);
            for (int i = 0; i < question.results[0].incorrect_answers.Count; i++)
            {
                question.results[0].incorrect_answers[i] = HttpUtility.HtmlDecode(question.results[0].incorrect_answers[i]);
            }
            // add the correct answer to the list of answers
            answers.AddFirst(question.results[0].correct_answer);
            // add the incorrect answers to the list of answers
            for(int i = 0; i < question.results[0].incorrect_answers.Count; i++)
            {
                answers.AddLast(question.results[0].incorrect_answers[i]);
            }
            // store the number of elements stored in the answers list
            answerCout = answers.Count;
            // mix up the elements stored in the answers list and add them to the answersMixed queue
            for (int i = 0; i < answerCout; i++)
            {
                // get a random int based on the total number of elements in the answers list
                randomIndex = _.random.Next(answers.Count);
                // get the value of the node at the random int
                node = answers.ElementAt<string>(randomIndex);
                // remove the node that contains the string
                answers.Remove(node);
                // add the string to the queue
                answersMixed.Enqueue(node);
            }
            // reset the time up timer
            _.timeIsUp = false;
            Program.ResetTimer();
            // output the question
            Console.WriteLine(question.results[0].question+"\n");
            // output all of the mixed answers
            index = 1;
            foreach(string answer in answersMixed)
            {
                Console.WriteLine("("+ index +") "+answer);
                index++;
            }
            // because im bad at trivia
            //Console.WriteLine("(Correct) " + question.results[0].correct_answer);
            // get the answer from the player
            playerAnswer = Console.ReadLine();
            if(!playerAnswer.Equals("1") || playerAnswer.Equals("2") || playerAnswer.Equals("3") || playerAnswer.Equals("4"))
            {
                Console.WriteLine("Its your life your playing with.\n");
            }
            //figure out which index is the right answer
            indexToCheck = 0;
            foreach (string answer in answersMixed)
            {
                indexToCheck++;
                if (answer.Contains(question.results[0].correct_answer))
                {
                    break;
                }
            }
            // if the player has runout of time then force their answer to be wrong
            if (_.timeIsUp)
            {
                playerAnswer = question.results[0].incorrect_answers[0];
            }
            // if the player enters a phrase containing the correct answer case insensitive
            if (playerAnswer.Equals("" + indexToCheck))
            {
                Console.WriteLine();
                toReturn = amount;
                // output a story element
                Console.WriteLine(story.AnswerWasRight(_.random.Next(4)));
            }
            // if the player enters a phrase containing the incorrect answer
            else
            {
                Console.WriteLine();
                // negate the amount
                toReturn = -amount;
                if (!_.timeIsUp)
                {
                    // output a story element
                    Console.WriteLine(story.AnswerWasWrong(_.random.Next(4)));
                }
                // if the player ran out of time
                else
                {
                    // output a story element
                    Console.WriteLine(story.RanOutOfTime(_.random.Next(4)));
                }
            }
            _.timer.Stop();
            return toReturn;
        }
        /* Method: TakeWalkDamage
         * Purpose: Remove health based on the weight of the direction chosen
         * Restrictions: None
         */
        public void TakeWalkDamage(Direction dir)
        {
            position = dir.Destination;
            health -= dir.Weight;
            // output a story element
            Console.WriteLine(story.WalkDamage(_.random.Next(4)));
            // Note: random damage must always come after walk damage or the player could die
            TakeRandomDamage();
            if (_.playersOldPosition != Position)
            {
                // output the description of the room
                Console.WriteLine(story.RoomDescription(Position));
                _.playersOldPosition = Position;
            }
        }
        /* Method: TakeRandomDamage
         * Purpose: Remove a random amount of health
         * Restrictions: None
         */
        public void TakeRandomDamage()
        {
            // if the room is not A or H
            if (position != _.start && position != _.goal)
            {
                // remove between up to all but one point of health from the player
                health -= _.random.Next(1, health);
                // if for any reason the player health drops below 1 enforce the health to be 1
                if (Health <= 0)
                {
                    health = 1;
                }
                // output a story element
                Console.WriteLine(story.RandomDamage(_.random.Next(4)));
            }
        }
        /* Method: DeadCheck
         * Purpose: To set the gameOver trigger to true
         * Restrictions: None
         */
        public void DeadCheck()
        {
            if(health <= 0)
            {
                _.gameOver = true;
                Console.WriteLine(story.Death);
            }
        }
    }
    /* Class: Program
     * Author: Jonathan Karcher
     * Purpose: Main entery class for the program
     */
    class Program
    {
        /* Method: Main
         * Purpose: Main entery point for the program
         * Restrictions: None
         */
        static void Main(string[] args)
        {
            // create a new story
            Story story = new Story();
            // create a new player
            Player player = new Player(_.start, story);

            // Dont Die data in matrix form
            //          [destination]
            // [origin] north, south, east, west
            (string, int)[,] adjacencyMatrix =
            /*  *//*  A*//*       B*//*     C*//*       D*//*       E*//*       F*//*       G*//*        H*/
            /*A*/{ {("NE", 0),  ("S", 2),  (null,-1), (null,-1),  (null,-1),  (null,-1),  (null,-1),  (null,-1) },
            /*B  */{(null,-1),  (null,-1), ("S", 2),  ("E", 3),   (null,-1),  (null,-1),  (null,-1),  (null,-1) },
            /*C  */{(null,-1),  ("N", 2),  (null,-1), (null,-1),  (null,-1),  (null,-1),  (null,-1),  ("S",20) },
            /*D  */{(null,-1),  ("W", 3),  ("S", 5),  (null,-1),  ("N", 2),   ("E", 4),   (null,-1),  (null,-1) },
            /*E  */{(null,-1),  (null,-1), (null,-1), (null,-1),  (null,-1),  ("S", 3),   (null,-1),  (null,-1) },
            /*F  */{(null,-1),  (null,-1), (null,-1), (null,-1),  (null,-1),  (null,-1),  ("E", 1),    (null,-1) },
            /*G  */{(null,-1),  (null,-1), (null,-1), (null,-1),  ("N", 0),   (null,-1),  (null,-1),  ("S", 2) },
            /*H  */{(null,-1),  (null,-1), (null,-1), (null,-1),  (null,-1),  (null,-1),  (null,-1),  (null,-1) } };

            // how to play
            Console.WriteLine(story.HowToPlay);
            Console.WriteLine("(press enter to continue)");
            Console.ReadLine();
            // output an initial greeting
            Console.WriteLine(story.Greeting);
            Console.WriteLine(story.RoomDescription(_.start));
            // as long as the player is alive
            while (!_.gameOver)
            {
                // move the player
                player.Move(adjacencyMatrix, _.goal);
                // check if the player has died at any point durring the move
                player.DeadCheck();
                // if the player has died or survived manage a replay option
                if (player.Position == _.goal)
                {
                    _.gameOver = true;
                }
                if(_.gameOver)
                {
                    string input = "";
                    Console.WriteLine("\nWould you like to play again? (Yes) (No)");
                    input = Console.ReadLine().ToUpper();
                    if (input.Equals(""))
                    {
                        input = "_";
                    }
                    // if they want to play again
                    if (input[0] == 'Y')
                    {
                        // reset the game
                        player.ResetPlayer();
                    }
                }
            }
        }
        /* Method: ResetTimer
         * Purpose: Reset the game timer to 15 seconds
         * Restrictions: None
         */
        public static void ResetTimer()
        {
            // set the timer to 15 seconds
            _.timer = new Timer(15000);
            // create an event handeler
            ElapsedEventHandler elapsedEventHandler = new ElapsedEventHandler(TimeIsUp);
            // add the event handeler to the elapsed listener
            _.timer.Elapsed += elapsedEventHandler;
            // start the timer
            _.timer.Start();
        }
        /* Method: TimeIsUp
         * Purpose: Inform the player that they have run out of time, tell the player how to proceed
         * Restrictions: None
         */
        static void TimeIsUp(Object source, ElapsedEventArgs e)
        {
            // tell the user the time is up
            Console.WriteLine("Time's up!");
            // tell the user how to proceed
            Console.WriteLine("(please press enter)");
            // set the timer trigger to be over
            _.timeIsUp = true;
            // stop the timer 
            _.timer.Stop();
        }
    }
}
