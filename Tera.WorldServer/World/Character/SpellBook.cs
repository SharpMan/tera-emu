using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.World.Character
{
    public class SpellBook
    {
        public const string SPELL_POSITION_CHAR = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

        /// <summary>
        /// Infos sur le sort
        /// </summary>
        public class SpellInfo
        {
            public int Id;
            public int Level;
            public int Position;
            public SpellLevel SpellLevel;

            public SpellInfo(int Id, int Level = 1, int Position = 25)
            {
                this.Id = Id;
                this.Level = Level;
                this.Position = Position;

                if (this.Level > 0)
                {
                    this.SpellLevel = SpellTable.Cache[Id].GetLevel(this.Level);
                    this.SpellLevel.Initialize();
                }
            }

            public void LevelUp()
            {
                if (this.Level < 6)
                {
                    this.Level++;

                    this.SpellLevel = SpellTable.Cache[Id].GetLevel(this.Level);
                    this.SpellLevel.Initialize();
                }
            }

            public void ToDatabase(BinaryWriter Writer)
            {
                Writer.Write(this.Id);
                Writer.Write(this.Level);
                Writer.Write(this.Position);
            }

            public String ToDatabase()
            {
                return Id + ";" + Level + ";" + Position;
            }

            public static SpellInfo FromDatabase(BinaryReader Reader)
            {
                return new SpellInfo(Reader.ReadInt32(), Reader.ReadInt32(), Reader.ReadInt32());
            }
        }

        private Dictionary<int, SpellInfo> mySpells = new Dictionary<int, SpellInfo>();

        #region Methods

        public static SpellBook GenerateForGuild()
        {
            var Book = new SpellBook();

            Book.AddSpell(462, 0);
            Book.AddSpell(461, 0);
            Book.AddSpell(460, 0);
            Book.AddSpell(459, 0);
            Book.AddSpell(458, 0);
            Book.AddSpell(457, 0);
            Book.AddSpell(456, 0);
            Book.AddSpell(455, 0);
            Book.AddSpell(454, 0);
            Book.AddSpell(453, 0);
            Book.AddSpell(452, 0);
            Book.AddSpell(451, 0);

            return Book;
        }

        public static SpellBook GenerateForPrisme()
        {
            var Book = new SpellBook();

            Book.AddSpell(56, 6);
            Book.AddSpell(24, 6);
            Book.AddSpell(157, 6);
            Book.AddSpell(63, 6);
            Book.AddSpell(8, 6);
            Book.AddSpell(81, 6);

            return Book;
        }

        public static SpellBook GenerateForBreed(ClassEnum Class)
        {
            var Book = new SpellBook();

            switch (Class)
            {
                case ClassEnum.CLASS_FECA:
                    Book.AddSpell(3, Position: 1);
                    Book.AddSpell(6, Position: 2);
                    Book.AddSpell(17, Position: 3);
                    break;

                case ClassEnum.CLASS_OSAMODAS:
                    Book.AddSpell(34, Position: 1);
                    Book.AddSpell(21, Position: 2);
                    Book.AddSpell(23, Position: 3);
                    break;

                case ClassEnum.CLASS_ENUTROF:
                    Book.AddSpell(51, Position: 1);
                    Book.AddSpell(43, Position: 2);
                    Book.AddSpell(41, Position: 3);
                    break;

                case ClassEnum.CLASS_SRAM:
                    Book.AddSpell(61, Position: 1);
                    Book.AddSpell(72, Position: 2);
                    Book.AddSpell(65, Position: 3);
                    break;

                case ClassEnum.CLASS_XELOR:
                    Book.AddSpell(82, Position: 1);
                    Book.AddSpell(81, Position: 2);
                    Book.AddSpell(83, Position: 3);
                    break;

                case ClassEnum.CLASS_ECAFLIP:
                    Book.AddSpell(102, Position: 1);
                    Book.AddSpell(103, Position: 2);
                    Book.AddSpell(105, Position: 3);
                    break;

                case ClassEnum.CLASS_ENIRIPSA:
                    Book.AddSpell(125, Position: 1);
                    Book.AddSpell(128, Position: 2);
                    Book.AddSpell(121, Position: 3);
                    break;

                case ClassEnum.CLASS_IOP:
                    Book.AddSpell(143, Position: 1);
                    Book.AddSpell(141, Position: 2);
                    Book.AddSpell(142, Position: 3);
                    break;

                case ClassEnum.CLASS_CRA:
                    Book.AddSpell(161, Position: 1);
                    Book.AddSpell(169, Position: 2);
                    Book.AddSpell(164, Position: 3);
                    break;

                case ClassEnum.CLASS_SADIDA:
                    Book.AddSpell(183, Position: 1);
                    Book.AddSpell(200, Position: 2);
                    Book.AddSpell(193, Position: 3);
                    break;

                case ClassEnum.CLASS_SACRIEUR:
                    Book.AddSpell(432, Position: 1);
                    Book.AddSpell(431, Position: 2);
                    Book.AddSpell(434, Position: 3);
                    break;

                case ClassEnum.CLASS_PANDAWA:
                    Book.AddSpell(686, Position: 1);
                    Book.AddSpell(692, Position: 2);
                    Book.AddSpell(687, Position: 3);
                    break;
            }

            return Book;
        }

        public void GenerateLevelUpSpell(ClassEnum Class, int Level)
        {
            switch (Class)
            {
                case ClassEnum.CLASS_FECA:
                    if (Level == 3)
                        this.AddSpell(4);//Renvoie de sort
                    if (Level == 6)
                        this.AddSpell(2);//Aveuglement
                    if (Level == 9)
                        this.AddSpell(1);//Armure Incandescente
                    if (Level == 13)
                        this.AddSpell(9);//Attaque nuageuse
                    if (Level == 17)
                        this.AddSpell(18);//Armure Aqueuse
                    if (Level == 21)
                        this.AddSpell(20);//Immunit�
                    if (Level == 26)
                        this.AddSpell(14);//Armure Venteuse
                    if (Level == 31)
                        this.AddSpell(19);//Bulle
                    if (Level == 36)
                        this.AddSpell(5);//Tr�ve
                    if (Level == 42)
                        this.AddSpell(16);//Science du b�ton
                    if (Level == 48)
                        this.AddSpell(8);//Retour du b�ton
                    if (Level == 54)
                        this.AddSpell(12);//glyphe d'Aveuglement
                    if (Level == 60)
                        this.AddSpell(11);//T�l�portation
                    if (Level == 70)
                        this.AddSpell(10);//Glyphe Enflamm�
                    if (Level == 80)
                        this.AddSpell(7);//Bouclier F�ca
                    if (Level == 90)
                        this.AddSpell(15);//Glyphe d'Immobilisation
                    if (Level == 100)
                        this.AddSpell(13);//Glyphe de Silence
                    if (Level == 200)
                        this.AddSpell(1901);//Invocation de Dopeul F�ca
                    break;

                case ClassEnum.CLASS_OSAMODAS:
                    if (Level == 3)
                        this.AddSpell(26);//B�n�diction Animale
                    if (Level == 6)
                        this.AddSpell(22);//D�placement F�lin
                    if (Level == 9)
                        this.AddSpell(35);//Invocation de Bouftou
                    if (Level == 13)
                        this.AddSpell(28);//Crapaud
                    if (Level == 17)
                        this.AddSpell(37);//Invocation de Prespic
                    if (Level == 21)
                        this.AddSpell(30);//Fouet
                    if (Level == 26)
                        this.AddSpell(27);//Piq�re Motivante
                    if (Level == 31)
                        this.AddSpell(24);//Corbeau
                    if (Level == 36)
                        this.AddSpell(33);//Griffe Cinglante
                    if (Level == 42)
                        this.AddSpell(25);//Soin Animal
                    if (Level == 48)
                        this.AddSpell(38);//Invocation de Sanglier
                    if (Level == 54)
                        this.AddSpell(36);//Frappe du Craqueleur
                    if (Level == 60)
                        this.AddSpell(32);//R�sistance Naturelle
                    if (Level == 70)
                        this.AddSpell(29);//Crocs du Mulou
                    if (Level == 80)
                        this.AddSpell(39);//Invocation de Bwork Mage
                    if (Level == 90)
                        this.AddSpell(40);//Invocation de Craqueleur
                    if (Level == 100)
                        this.AddSpell(31);//Invocation de Dragonnet Rouge
                    if (Level == 200)
                        this.AddSpell(1902);//Invocation de Dopeul Osamodas
                    break;

                case ClassEnum.CLASS_ENUTROF:
                    if (Level == 3)
                        this.AddSpell(49);//Pelle Fantomatique
                    if (Level == 6)
                        this.AddSpell(42);//Chance
                    if (Level == 9)
                        this.AddSpell(47);//Bo�te de Pandore
                    if (Level == 13)
                        this.AddSpell(48);//Remblai
                    if (Level == 17)
                        this.AddSpell(45);//Cl� R�ductrice
                    if (Level == 21)
                        this.AddSpell(53);//Force de l'Age
                    if (Level == 26)
                        this.AddSpell(46);//D�sinvocation
                    if (Level == 31)
                        this.AddSpell(52);//Cupidit�
                    if (Level == 36)
                        this.AddSpell(44);//Roulage de Pelle
                    if (Level == 42)
                        this.AddSpell(50);//Maladresse
                    if (Level == 48)
                        this.AddSpell(54);//Maladresse de Masse
                    if (Level == 54)
                        this.AddSpell(55);//Acc�l�ration
                    if (Level == 60)
                        this.AddSpell(56);//Pelle du Jugement
                    if (Level == 70)
                        this.AddSpell(58);//Pelle Massacrante
                    if (Level == 80)
                        this.AddSpell(59);//Corruption
                    if (Level == 90)
                        this.AddSpell(57);//Pelle Anim�e
                    if (Level == 100)
                        this.AddSpell(60);//Coffre Anim�
                    if (Level == 200)
                        this.AddSpell(1903);//Invocation de Dopeul Enutrof
                    break;

                case ClassEnum.CLASS_SRAM:
                    if (Level == 3)
                        this.AddSpell(66);//Poison insidieux
                    if (Level == 6)
                        this.AddSpell(68);//Fourvoiement
                    if (Level == 9)
                        this.AddSpell(63);//Coup Sournois
                    if (Level == 13)
                        this.AddSpell(74);//Double
                    if (Level == 17)
                        this.AddSpell(64);//Rep�rage
                    if (Level == 21)
                        this.AddSpell(79);//Pi�ge de Masse
                    if (Level == 26)
                        this.AddSpell(78);//Invisibilit� d'Autrui
                    if (Level == 31)
                        this.AddSpell(71);//Pi�ge Empoisonn�
                    if (Level == 36)
                        this.AddSpell(62);//Concentration de Chakra
                    if (Level == 42)
                        this.AddSpell(69);//Pi�ge d'Immobilisation
                    if (Level == 48)
                        this.AddSpell(77);//Pi�ge de Silence
                    if (Level == 54)
                        this.AddSpell(73);//Pi�ge r�pulsif
                    if (Level == 60)
                        this.AddSpell(67);//Peur
                    if (Level == 70)
                        this.AddSpell(70);//Arnaque
                    if (Level == 80)
                        this.AddSpell(75);//Pulsion de Chakra
                    if (Level == 90)
                        this.AddSpell(76);//Attaque Mortelle
                    if (Level == 100)
                        this.AddSpell(80);//Pi�ge Mortel
                    if (Level == 200)
                        this.AddSpell(1904);//Invocation de Dopeul Sram
                    break;

                case ClassEnum.CLASS_XELOR:
                    if (Level == 3)
                        this.AddSpell(84);//Gelure
                    if (Level == 6)
                        this.AddSpell(100);//Sablier de X�lor
                    if (Level == 9)
                        this.AddSpell(92);//Rayon Obscur
                    if (Level == 13)
                        this.AddSpell(88);//T�l�portation
                    if (Level == 17)
                        this.AddSpell(93);//Fl�trissement
                    if (Level == 21)
                        this.AddSpell(85);//Flou
                    if (Level == 26)
                        this.AddSpell(96);//Poussi�re Temporelle
                    if (Level == 31)
                        this.AddSpell(98);//Vol du Temps
                    if (Level == 36)
                        this.AddSpell(86);//Aiguille Chercheuse
                    if (Level == 42)
                        this.AddSpell(89);//D�vouement
                    if (Level == 48)
                        this.AddSpell(90);//Fuite
                    if (Level == 54)
                        this.AddSpell(87);//D�motivation
                    if (Level == 60)
                        this.AddSpell(94);//Protection Aveuglante
                    if (Level == 70)
                        this.AddSpell(99);//Momification
                    if (Level == 80)
                        this.AddSpell(95);//Horloge
                    if (Level == 90)
                        this.AddSpell(91);//Frappe de X�lor
                    if (Level == 100)
                        this.AddSpell(97);//Cadran de X�lor
                    if (Level == 200)
                        this.AddSpell(1905);//Invocation de Dopeul X�lor
                    break;

                case ClassEnum.CLASS_ECAFLIP:
                    if (Level == 3)
                        this.AddSpell(109);//Bluff
                    if (Level == 6)
                        this.AddSpell(113);//Perception
                    if (Level == 9)
                        this.AddSpell(111);//Contrecoup
                    if (Level == 13)
                        this.AddSpell(104);//Tr�fle
                    if (Level == 17)
                        this.AddSpell(119);//Tout ou rien
                    if (Level == 21)
                        this.AddSpell(101);//Roulette
                    if (Level == 26)
                        this.AddSpell(107);//Topkaj
                    if (Level == 31)
                        this.AddSpell(116);//Langue R�peuse
                    if (Level == 36)
                        this.AddSpell(106);//Roue de la Fortune
                    if (Level == 42)
                        this.AddSpell(117);//Griffe Invocatrice
                    if (Level == 48)
                        this.AddSpell(108);//Esprit F�lin
                    if (Level == 54)
                        this.AddSpell(115);//Odorat
                    if (Level == 60)
                        this.AddSpell(118);//R�flexes
                    if (Level == 70)
                        this.AddSpell(110);//Griffe Joueuse
                    if (Level == 80)
                        this.AddSpell(112);//Griffe de Ceangal
                    if (Level == 90)
                        this.AddSpell(114);//Rekop
                    if (Level == 100)
                        this.AddSpell(120);//Destin d'Ecaflip
                    if (Level == 200)
                        this.AddSpell(1906);//Invocation de Dopeul Ecaflip
                    break;

                case ClassEnum.CLASS_ENIRIPSA:
                    if (Level == 3)
                        this.AddSpell(124);//Mot Soignant
                    if (Level == 6)
                        this.AddSpell(122);//Mot Blessant
                    if (Level == 9)
                        this.AddSpell(126);//Mot Stimulant
                    if (Level == 13)
                        this.AddSpell(127);//Mot de Pr�vention
                    if (Level == 17)
                        this.AddSpell(123);//Mot Drainant
                    if (Level == 21)
                        this.AddSpell(130);//Mot Revitalisant
                    if (Level == 26)
                        this.AddSpell(131);//Mot de R�g�n�ration
                    if (Level == 31)
                        this.AddSpell(132);//Mot d'Epine
                    if (Level == 36)
                        this.AddSpell(133);//Mot de Jouvence
                    if (Level == 42)
                        this.AddSpell(134);//Mot Vampirique
                    if (Level == 48)
                        this.AddSpell(135);//Mot de Sacrifice
                    if (Level == 54)
                        this.AddSpell(129);//Mot d'Amiti�
                    if (Level == 60)
                        this.AddSpell(136);//Mot d'Immobilisation
                    if (Level == 70)
                        this.AddSpell(137);//Mot d'Envol
                    if (Level == 80)
                        this.AddSpell(138);//Mot de Silence
                    if (Level == 90)
                        this.AddSpell(139);//Mot d'Altruisme
                    if (Level == 100)
                        this.AddSpell(140);//Mot de Reconstitution
                    if (Level == 200)
                        this.AddSpell(1907);//Invocation de Dopeul Eniripsa
                    break;

                case ClassEnum.CLASS_IOP:
                    if (Level == 3)
                        this.AddSpell(144);//Compulsion
                    if (Level == 6)
                        this.AddSpell(145);//Ep�e Divine
                    if (Level == 9)
                        this.AddSpell(146);//Ep�e du Destin
                    if (Level == 13)
                        this.AddSpell(147);//Guide de Bravoure
                    if (Level == 17)
                        this.AddSpell(148);//Amplification
                    if (Level == 21)
                        this.AddSpell(154);//Ep�e Destructrice
                    if (Level == 26)
                        this.AddSpell(150);//Couper
                    if (Level == 31)
                        this.AddSpell(151);//Souffle
                    if (Level == 36)
                        this.AddSpell(155);//Vitalit�
                    if (Level == 42)
                        this.AddSpell(152);//Ep�e du Jugement
                    if (Level == 48)
                        this.AddSpell(153);//Puissance
                    if (Level == 54)
                        this.AddSpell(149);//Mutilation
                    if (Level == 60)
                        this.AddSpell(156);//Temp�te de Puissance
                    if (Level == 70)
                        this.AddSpell(157);//Ep�e C�leste
                    if (Level == 80)
                        this.AddSpell(158);//Concentration
                    if (Level == 90)
                        this.AddSpell(160);//Ep�e de Iop
                    if (Level == 100)
                        this.AddSpell(159);//Col�re de Iop
                    if (Level == 200)
                        this.AddSpell(1908);//Invocation de Dopeul Iop
                    break;

                case ClassEnum.CLASS_CRA:
                    if (Level == 3)
                        this.AddSpell(163);//Fl�che Glac�e
                    if (Level == 6)
                        this.AddSpell(165);//Fl�che enflamm�e
                    if (Level == 9)
                        this.AddSpell(172);//Tir Eloign�
                    if (Level == 13)
                        this.AddSpell(167);//Fl�che d'Expiation
                    if (Level == 17)
                        this.AddSpell(168);//Oeil de Taupe
                    if (Level == 21)
                        this.AddSpell(162);//Tir Critique
                    if (Level == 26)
                        this.AddSpell(170);//Fl�che d'Immobilisation
                    if (Level == 31)
                        this.AddSpell(171);//Fl�che Punitive
                    if (Level == 36)
                        this.AddSpell(166);//Tir Puissant
                    if (Level == 42)
                        this.AddSpell(173);//Fl�che Harcelante
                    if (Level == 48)
                        this.AddSpell(174);//Fl�che Cinglante
                    if (Level == 54)
                        this.AddSpell(176);//Fl�che Pers�cutrice
                    if (Level == 60)
                        this.AddSpell(175);//Fl�che Destructrice
                    if (Level == 70)
                        this.AddSpell(178);//Fl�che Absorbante
                    if (Level == 80)
                        this.AddSpell(177);//Fl�che Ralentissante
                    if (Level == 90)
                        this.AddSpell(179);//Fl�che Explosive
                    if (Level == 100)
                        this.AddSpell(180);//Ma�trise de l'Arc
                    if (Level == 200)
                        this.AddSpell(1909);//Invocation de Dopeul Cra
                    break;

                case ClassEnum.CLASS_SADIDA:
                    if (Level == 3)
                        this.AddSpell(198);//Sacrifice Poupesque
                    if (Level == 6)
                        this.AddSpell(195);//Larme
                    if (Level == 9)
                        this.AddSpell(182);//Invocation de la Folle
                    if (Level == 13)
                        this.AddSpell(192);//Ronce Apaisante
                    if (Level == 17)
                        this.AddSpell(197);//Puissance Sylvestre
                    if (Level == 21)
                        this.AddSpell(189);//Invocation de la Sacrifi�e
                    if (Level == 26)
                        this.AddSpell(181);//Tremblement
                    if (Level == 31)
                        this.AddSpell(199);//Connaissance des Poup�es
                    if (Level == 36)
                        this.AddSpell(191);//Ronce Multiples
                    if (Level == 42)
                        this.AddSpell(186);//Arbre
                    if (Level == 48)
                        this.AddSpell(196);//Vent Empoisonn�
                    if (Level == 54)
                        this.AddSpell(190);//Invocation de la Gonflable
                    if (Level == 60)
                        this.AddSpell(194);//Ronces Agressives
                    if (Level == 70)
                        this.AddSpell(185);//Herbe Folle
                    if (Level == 80)
                        this.AddSpell(184);//Feu de Brousse
                    if (Level == 90)
                        this.AddSpell(188);//Ronce Insolente
                    if (Level == 100)
                        this.AddSpell(187);//Invocation de la Surpuissante
                    if (Level == 200)
                        this.AddSpell(1910);//Invocation de Dopeul Sadida
                    break;

                case ClassEnum.CLASS_SACRIEUR:
                    if (Level == 3)
                        this.AddSpell(444);//D�robade
                    if (Level == 6)
                        this.AddSpell(449);//D�tour
                    if (Level == 9)
                        this.AddSpell(436);//Assaut
                    if (Level == 13)
                        this.AddSpell(437);//Ch�timent Agile
                    if (Level == 17)
                        this.AddSpell(439);//Dissolution
                    if (Level == 21)
                        this.AddSpell(433);//Ch�timent Os�
                    if (Level == 26)
                        this.AddSpell(443);//Ch�timent Spirituel
                    if (Level == 31)
                        this.AddSpell(440);//Sacrifice
                    if (Level == 36)
                        this.AddSpell(442);//Absorption
                    if (Level == 42)
                        this.AddSpell(441);//Ch�timent Vilatesque
                    if (Level == 48)
                        this.AddSpell(445);//Coop�ration
                    if (Level == 54)
                        this.AddSpell(438);//Transposition
                    if (Level == 60)
                        this.AddSpell(446);//Punition
                    if (Level == 70)
                        this.AddSpell(447);//Furie
                    if (Level == 80)
                        this.AddSpell(448);//Ep�e Volante
                    if (Level == 90)
                        this.AddSpell(435);//Tansfert de Vie
                    if (Level == 100)
                        this.AddSpell(450);//Folie Sanguinaire
                    if (Level == 200)
                        this.AddSpell(1911);//Invocation de Dopeul Sacrieur
                    break;

                case ClassEnum.CLASS_PANDAWA:
                    if (Level == 3)
                        this.AddSpell(689);//Epouvante
                    if (Level == 6)
                        this.AddSpell(690);//Souffle Alcoolis�
                    if (Level == 9)
                        this.AddSpell(691);//Vuln�rabilit� Aqueuse
                    if (Level == 13)
                        this.AddSpell(688);//Vuln�rabilit� Incandescente
                    if (Level == 17)
                        this.AddSpell(693);//Karcham
                    if (Level == 21)
                        this.AddSpell(694);//Vuln�rabilit� Venteuse
                    if (Level == 26)
                        this.AddSpell(695);//Stabilisation
                    if (Level == 31)
                        this.AddSpell(696);//Chamrak
                    if (Level == 36)
                        this.AddSpell(697);//Vuln�rabilit� Terrestre
                    if (Level == 42)
                        this.AddSpell(698);//Souillure
                    if (Level == 48)
                        this.AddSpell(699);//Lait de Bambou
                    if (Level == 54)
                        this.AddSpell(700);//Vague � Lame
                    if (Level == 60)
                        this.AddSpell(701);//Col�re de Zato�shwan
                    if (Level == 70)
                        this.AddSpell(702);//Flasque Explosive
                    if (Level == 80)
                        this.AddSpell(703);//Pandatak
                    if (Level == 90)
                        this.AddSpell(704);//Pandanlku
                    if (Level == 100)
                        this.AddSpell(705);//Lien Spiritueux
                    if (Level == 200)
                        this.AddSpell(1912);//Invocation de Dopeul Pandawa
                    break;
            }
        }

        public void AddSpell(int SpellId, int Level = 1, int Position = 25, WorldClient Client = null)
        {
            lock (this.mySpells)
                if (!this.mySpells.ContainsKey(SpellId))
                {
                    this.mySpells.Add(SpellId, new SpellInfo(SpellId, Level, Position));
                    if (Client != null)
                    {
                        Client.Send(new SpellsListMessage(Client.Character));
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO,3,SpellId.ToString()));
                    }
                     
                }
        }

        public bool HasSpell(int SpellId)
        {
            return this.mySpells.ContainsKey(SpellId);
        }

        public void LevelUpSepll(int SpellId)
        {
            lock (this.mySpells)
                if (this.mySpells.ContainsKey(SpellId))
                    this.mySpells[SpellId].LevelUp();
        }

        public SpellLevel GetSpellLevel(int SpellId)
        {
            lock (this.mySpells)
                if (this.mySpells.ContainsKey(SpellId))
                    return this.mySpells[SpellId].SpellLevel;

            return null;
        }

        public void MoveSpell(int SpellId, int Position)
        {
            lock (this.mySpells)
            {
                foreach (var Spell in this.mySpells.Values)
                    if (Spell.Position == Position)
                        Spell.Position = 25;

                this.mySpells[SpellId].Position = Position;
            }
        }

        public List<SpellLevel> GetSpells()
        {
            return this.mySpells.Values.Select(x => x.SpellLevel).ToList();
        }

        public IEnumerable GetSpellInfos()
        {
            return this.mySpells.Values;
        }

        public Dictionary<int, SpellInfo> GetMySpells()
        {
            return this.mySpells;
        }

        public void SerializeAsSpellsListMessage(StringBuilder Packet)
        {
            lock (this.mySpells)
            {
                foreach (var Spell in this.mySpells.Values)
                {
                    Packet.Append(Spell.Id);
                    Packet.Append('~');
                    Packet.Append(Spell.Level);
                    Packet.Append('~');
                    Packet.Append(SpellBook.SPELL_POSITION_CHAR[Spell.Position]);
                    Packet.Append(';');
                }
            }
        }

        #endregion

        #region Save/Load

        public String ToDatabase()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var Spell in this.mySpells.Values)
            {
                sb.Append(Spell.ToDatabase());
                sb.Append(",");
            }
            return sb.ToString();
        }


        public static SpellBook FromDatabase(String Datas)
        {
            var SpellBook = new SpellBook();
            String[] spells = Datas.Split(',');
            foreach (string reader in spells)
            {
                if (String.IsNullOrEmpty(reader) || reader == "")
                {
                    continue;
                }
                try
                {
                    int id = int.Parse(reader.Split(';')[0]);
                    int lvl = int.Parse(reader.Split(';')[1]);
                    int place = int.Parse(reader.Split(';')[2]);
                    var Spell = new SpellInfo(id,lvl,place);
                    SpellBook.mySpells.Add(Spell.Id, Spell);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            return SpellBook;
        }

        #endregion

    }
}
