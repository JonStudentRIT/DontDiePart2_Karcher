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
        public static int goal = _.H;
        public static Random random = new Random();
        public static bool timeIsUp = false;
        public static Timer timer = new Timer();
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
        /// <summary>
        /// The voice is based on a combination of SCP-575 and nyarthotep
        /// SCP-575 is a formless monster that hunts in the darkness
        /// Nyarthotep is a "son" of yog sothoth and the only lovecraftian-eldritch god that enjoys a one on one experience with humans
        /// Every room has a reference to either an SCP or a lovcraftian myth
        /// fair warning: if you dont like the horror genere then let the wierd things just be wierd
        /// </summary>
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
            howToPlay = "How to play: As you play the game you will see the propts (Gamble) (North) (South) (East) (West).\n" +
                "Entering any of these values case insesitive, when the prompt is available will allow the game to proceed.\n" +
                "The goal of the game is to escape the labyrinth.\n" +
                "Your Health: is the amount of health you have left.\n" +
                "(Gamble) allows the player to gamble any amount of health up to the value displayed in \"Your Health:\".\n" +
                "You can only die while gambleing your life. You die when the value reaches 0.\n" +
                "Wagering your life requiers an integer value up to the value of health that the player has.\n" +
                "You will have 15 seconds to answer the question.\n" +
                "(North) Travel north, (South) travel south, (East) travel east, (West) travel west.\n" +
                "Traveling down any path will result in loss of health.\n" +
                "Acess to a room is only available if you have enough health to travel to it.\n";

            // initial greeting
            greeting = "A shrill high pitch voice anounces, \"Ahhh... it wakes, I was begining to get bored!\"\n" +
                "You call out, \"Whos there?\"\n" +
                "\"A game of riddles is what I sought, and should you fail your life be naught!\"\n" +
                "... clearly whoever is talking to you dosnt want to answer you question, but they arnt attacking you... yet, either.\n" +
                "You ask the darkness \"What are the rules?\"\n" +
                "\"The question, the game, the words to maim will flow forth from me to you, \nand to the dark one phrase you need, this I promise you.\"\n" +
                "... ok so it seems that there will be a question that I will need to answer,\"the words to maim\" seems to mean \nthat there are consiquences if im wrong.";

            // if you die
            death = "In an instant your body fails to respond to you but time dosnt seem to stop or even slow.\n" +
                "You lay on the floor for what seems like hours... maybe days, confused you can only wonder \"what happens after\".";
            
            // when asking a question
            askAQuestion[0] = "Ok voice in the dark im ready, ask your question.\n";
            askAQuestion[1] = "Are you there? Im ready for a question.\n";
            askAQuestion[2] = "Hey creapy crawller I dont have all day, make with the question.\n";
            askAQuestion[3] = "A shrill voice in the dark \"You have been quiet, are you ready to play?\" Yes, ask your question.\n";

            // if the answer was right
            answerWasRight[0] = "You feel imbued with new life as the voice cackles with joy \"Your right!\"... thats strange does it actually \nwant me to win.\n";
            answerWasRight[1] = "A wound you have been trying to ignore begins to rappidly heal as you hear the voice say \"Your right!\"\n";
            answerWasRight[2] = "A warm sensation envelopes you, the voice with a gleefull tone says \"Your right!\"\n";
            answerWasRight[3] = "You are filled with energy as the whole room seems to become brighter. The voice anounces \"Your right!\"\n";

            // if the answer was wrong
            answerWasWrong[0] = "You drop to your knee as you feel the energy leave your body, as you struggle to stay concious you hear the word \n\"Wrong.\"\n";
            answerWasWrong[1] = "A fit of coughing takes over as you feel all the air being forced from your lungs. The voice in its confusingly \nchearfull tone anounces \"Wrong.\"\n";
            answerWasWrong[2] = "In a flash a cut appears on your chest, you clutch the wound as you look for the source as the voice anounces \"Wrong.\"\n";
            answerWasWrong[3] = "For a moment your vision fades and in a panic you stumble and fall, hitting your head against the floor. The voice \nanounces \"Wrong.\"\n";

            // if the player runs out of time
            ranOutOfTime[0] = "A deep chuckle fills the room, different from the voice.  With a loud boom you hear \"Times Up.\"\n";
            ranOutOfTime[1] = "I stumped you this time didnt I... either way \"Times Up.\"\n";
            ranOutOfTime[2] = "Well well well it seems \"Times Up.\", if you keep this up you'll be in the bone yard before you know it.\n";
            ranOutOfTime[3] = "Im growing bored with your silence... its not wise to BORE me. \"Times Up.\"\n";

            // when walking from room to room
            walkDamage[0] = "The path seems to have been longer than you expected and your leggs ach as you come to the end of the path.\n";
            walkDamage[1] = "As you walk through the dark you trip over a rock and fall to the ground.\n";
            walkDamage[2] = "In the dark you misstep and trip over your own feet.\n";
            walkDamage[3] = "A rock falls from the stone cealing striking you on the head.\n";

            // when walking from room to room
            randomDamage[0] = "Leaping from the dark somthing small and furry claws at your ankles and flees off into the dark.\n";
            randomDamage[1] = "A rock falls from the stone cealing striking you on the head.\n";
            randomDamage[2] = "In the dark you misstep and trip over your own feet.\n";
            randomDamage[3] = "As you walk through the dark you trip over a rock and fall to the ground.\n";

            // the descriptions of the room
            roomDescription[_.A] = "As you look around, the rooom is dark with bairley enough light to see.  \nThe walls are slick though its too dark to tell with what, the floor seems to be of flatend stone and the cealling... \nif there is one, is as dark as the void. \n";
            roomDescription[_.B] = "You emerge in a dimly lit room with hardwood floors in the center a table with a candalabra \nin the shape of a man burns... you probably shouldnt touch it.  \nThe walls are painted in a substance theat shimmers like gold in the candlelight.  \n";
            roomDescription[_.C] = "The room seems to be a perfect dome with a mirror finish, at its center a fountan flows with a strange purple fluid. \n";
            roomDescription[_.D] = "As you enter the room you see bones scattered across the floor human and animal alike, it seems somthing was... \nor, is here and its not friendly \n";
            roomDescription[_.E] = "Your in a room... at least your pretty sure you are.  \nIts so dark you cant see anything except a faint light a path perhaps. \n";
            roomDescription[_.F] = "The walls are the color of blood and slick with somthing clear. \nThe floor seems to squish with every step... as you look closer you can see veins pulsing throughout the room.  \n";
            roomDescription[_.G] = "Its cold in this room, small puddles on the floor are frozen solid and snow falls from the blindingly wight cealling \nto create a sort of hourglass... you probably shouldnt linger or you may become trapped.  \n";
            roomDescription[_.H] = "The light... for the first time you can see the sun shine.  \nYou have survived the labyrinth.";
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
            position = _.A;
            health = 1;
        }
        /* Method: Move
         * Purpose: Update the player health and position based on the value entered
         * Restrictions: None
         */
        public void Move(Room room, int goal)
        {
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
            if(input.Equals(""))
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
                    if(int.TryParse(input, out amount))
                    {
                        // if they try to barter health that they dont have
                        if(amount > health)
                        {
                            Console.WriteLine("You dont have that much life to wager.\n");
                        }
                        // if they try to cheat the system by entering a negative number and intentionally getting questions wrong
                        if(amount < 0)
                        {
                            Console.WriteLine("HA, nice try but no, entering a negative value wont get you out of this one.\n");
                            // I think ill still alow a value of 0 so the player can try a few questions before risking death
                        }
                    }
                } while (!int.TryParse(input, out amount) || amount > health || amount < 0);
                health += GambleForHealth(amount);
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
            foreach(string answer in answersMixed)
            {
                Console.WriteLine(answer);
            }
            // get the answer from the player
            playerAnswer = Console.ReadLine();
            //playerAnswer = question.results[0].correct_answer;
            // if the player has runout of time then force their answer to be wrong
            if (_.timeIsUp)
            {
                playerAnswer = question.results[0].incorrect_answers[0];
            }
            // if the player enters a phrase containing the correct answer case insensitive
            if(playerAnswer.ToUpper().Contains(question.results[0].correct_answer.ToUpper()))
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
        }
        /* Method: TakeRandomDamage
         * Purpose: Remove a random amount of health
         * Restrictions: None
         */
        public void TakeRandomDamage()
        {
            // if the room is not A or H
            if (position != _.A && position != _.H)
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
            Player player = new Player(_.A, story);
            
            // Dont Die data using a dedicated class 
            // [origin] (direction(destination,weight))(...)(...)(...), description
            Room[] rooms = new Room[8];
            rooms[_.A] = new Room(new Direction(_.A, 0), new Direction(_.B, 2), new Direction(_.A, 0), null , story.RoomDescription(_.A));
            rooms[_.B] = new Room(null, new Direction(_.C, 2), new Direction(_.D, 3), null, story.RoomDescription(_.B));
            rooms[_.C] = new Room(new Direction(_.B, 2), new Direction(_.H, 20), null, null, story.RoomDescription(_.C));
            rooms[_.D] = new Room(new Direction(_.E, 2), new Direction(_.C, 5), new Direction(_.F, 4), new Direction(_.B, 3), story.RoomDescription(_.D));
            rooms[_.E] = new Room(null, new Direction(_.F, 3), null, null, story.RoomDescription(_.E));
            rooms[_.F] = new Room(null, null, new Direction(_.G, 1), null, story.RoomDescription(_.F));
            rooms[_.G] = new Room(new Direction(_.E, 0), new Direction(_.H, 2), null, null, story.RoomDescription(_.G));
            rooms[_.H] = new Room(null, null, null, null, story.RoomDescription(_.H));
            // how to play
            Console.WriteLine(story.HowToPlay);
            Console.WriteLine("(press enter to continue)");
            Console.ReadLine();
            // output an initial greeting
            Console.WriteLine(story.Greeting);
            // as long as the player is alive
            while (!_.gameOver)
            {
                // output the description of the room
                Console.WriteLine(rooms[player.Position].Description);
                // move the player
                player.Move(rooms[player.Position], _.goal);
                // check if the player has died at any point durring the move
                player.DeadCheck();
                // if the player has reached the goal
                if(player.Position == _.goal)
                {
                    Console.WriteLine(rooms[_.goal].Description);
                    _.gameOver = true;
                }
                // if the player has died manage a replay option
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
