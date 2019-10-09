using System;
using System.Collections.Generic;

namespace Xiropht_Rpc_Wallet.Object
{
    public class ClassBigTransaction 
    {
        private Dictionary<long, string> _bigDictionaryTransaction1; 
        private Dictionary<long, string> _bigDictionaryTransaction2; 
        private Dictionary<long, string> _bigDictionaryTransaction3; 
        private Dictionary<long, string> _bigDictionaryTransaction4; 
        private Dictionary<long, string> _bigDictionaryTransaction5;
        private Dictionary<long, string> _bigDictionaryTransaction6; // 99 999 999 - 59 999 999
        private Dictionary<long, string> _bigDictionaryTransaction7; // 59 999 999 - 69 999 999
        private Dictionary<long, string> _bigDictionaryTransaction8; // 79 999 999 - 89 999 999
        private Dictionary<long, string> _bigDictionaryTransaction9; // 89 999 999 - 99 999 999 
        private Dictionary<long, string> _bigDictionaryTransaction10; // 99 999 999 - 109 999 999 ~3.962 GB


        private Dictionary<long, string> _bigDictionaryTransaction11; // 109 999 999 - 119 999 999
        private Dictionary<long, string> _bigDictionaryTransaction12; // 119 999 999 - 129 999 999
        private Dictionary<long, string> _bigDictionaryTransaction13; // 129 999 999 - 139 999 999
        private Dictionary<long, string> _bigDictionaryTransaction14; // 139 999 999 - 149 999 999
        private Dictionary<long, string> _bigDictionaryTransaction15; // 149 999 999 - 159 999 999
        private Dictionary<long, string> _bigDictionaryTransaction16; // 159 999 999 - 169 999 999
        private Dictionary<long, string> _bigDictionaryTransaction17; // 169 999 999 - 179 999 999
        private Dictionary<long, string> _bigDictionaryTransaction18; // 179 999 999 - 189 999 999
        private Dictionary<long, string> _bigDictionaryTransaction19; // 189 999 999 - 199 999 999
        private Dictionary<long, string> _bigDictionaryTransaction20; // 199 999 999 - 209 999 999 ~7.924 GB

        private Dictionary<long, string> _bigDictionaryTransaction21; // 209 999 999 - 219 999 999
        private Dictionary<long, string> _bigDictionaryTransaction22; // 219 999 999 - 229 999 999
        private Dictionary<long, string> _bigDictionaryTransaction23; // 229 999 999 - 239 999 999
        private Dictionary<long, string> _bigDictionaryTransaction24; // 239 999 999 - 249 999 999
        private Dictionary<long, string> _bigDictionaryTransaction25; // 249 999 999 - 259 999 999
        private Dictionary<long, string> _bigDictionaryTransaction26; // 259 999 999 - 269 999 999
        private Dictionary<long, string> _bigDictionaryTransaction27; // 269 999 999 - 279 999 999
        private Dictionary<long, string> _bigDictionaryTransaction28; // 279 999 999 - 289 999 999
        private Dictionary<long, string> _bigDictionaryTransaction29; // 289 999 999 - 299 999 999
        private Dictionary<long, string> _bigDictionaryTransaction30; // 299 999 999 - 309 999 999 ~11,886 GB

        private Dictionary<long, string> _bigDictionaryTransaction31; // 309 999 999 - 319 999 999
        private Dictionary<long, string> _bigDictionaryTransaction32; // 319 999 999 - 329 999 999
        private Dictionary<long, string> _bigDictionaryTransaction33; // 329 999 999 - 339 999 999
        private Dictionary<long, string> _bigDictionaryTransaction34; // 339 999 999 - 349 999 999
        private Dictionary<long, string> _bigDictionaryTransaction35; // 349 999 999 - 359 999 999
        private Dictionary<long, string> _bigDictionaryTransaction36; // 359 999 999 - 369 999 999
        private Dictionary<long, string> _bigDictionaryTransaction37; // 369 999 999 - 379 999 999
        private Dictionary<long, string> _bigDictionaryTransaction38; // 379 999 999 - 389 999 999
        private Dictionary<long, string> _bigDictionaryTransaction39; // 389 999 999 - 399 999 999
        private Dictionary<long, string> _bigDictionaryTransaction40; // 399 999 999 - 409 999 999 ~15.848 GB

        private Dictionary<long, string> _bigDictionaryTransaction41; // 409 999 999 - 419 999 999
        private Dictionary<long, string> _bigDictionaryTransaction42; // 419 999 999 - 429 999 999
        private Dictionary<long, string> _bigDictionaryTransaction43; // 429 999 999 - 439 999 999
        private Dictionary<long, string> _bigDictionaryTransaction44; // 439 999 999 - 449 999 999
        private Dictionary<long, string> _bigDictionaryTransaction45; // 449 999 999 - 459 999 999
        private Dictionary<long, string> _bigDictionaryTransaction46; // 459 999 999 - 469 999 999
        private Dictionary<long, string> _bigDictionaryTransaction47; // 469 999 999 - 479 999 999
        private Dictionary<long, string> _bigDictionaryTransaction48; // 479 999 999 - 489 999 999
        private Dictionary<long, string> _bigDictionaryTransaction49; // 489 999 999 - 499 999 999
        private Dictionary<long, string> _bigDictionaryTransaction50; // 499 999 999 - 509 999 999 ~19.810 GB

        private Dictionary<long, string> _bigDictionaryTransaction51; // 509 999 999 - 519 999 999
        private Dictionary<long, string> _bigDictionaryTransaction52; // 519 999 999 - 529 999 999
        private Dictionary<long, string> _bigDictionaryTransaction53; // 529 999 999 - 539 999 999
        private Dictionary<long, string> _bigDictionaryTransaction54; // 539 999 999 - 549 999 999
        private Dictionary<long, string> _bigDictionaryTransaction55; // 549 999 999 - 559 999 999
        private Dictionary<long, string> _bigDictionaryTransaction56; // 559 999 999 - 569 999 999
        private Dictionary<long, string> _bigDictionaryTransaction57; // 569 999 999 - 579 999 999
        private Dictionary<long, string> _bigDictionaryTransaction58; // 579 999 999 - 589 999 999
        private Dictionary<long, string> _bigDictionaryTransaction59; // 589 999 999 - 599 999 999
        private Dictionary<long, string> _bigDictionaryTransaction60; // 599 999 999 - 609 999 999 ~23.772 GB

        private Dictionary<long, string> _bigDictionaryTransaction61; // 609 999 999 - 619 999 999
        private Dictionary<long, string> _bigDictionaryTransaction62; // 619 999 999 - 629 999 999
        private Dictionary<long, string> _bigDictionaryTransaction63; // 629 999 999 - 639 999 999
        private Dictionary<long, string> _bigDictionaryTransaction64; // 639 999 999 - 649 999 999
        private Dictionary<long, string> _bigDictionaryTransaction65; // 649 999 999 - 659 999 999
        private Dictionary<long, string> _bigDictionaryTransaction66; // 659 999 999 - 669 999 999
        private Dictionary<long, string> _bigDictionaryTransaction67; // 669 999 999 - 679 999 999
        private Dictionary<long, string> _bigDictionaryTransaction68; // 679 999 999 - 689 999 999
        private Dictionary<long, string> _bigDictionaryTransaction69; // 689 999 999 - 699 999 999
        private Dictionary<long, string> _bigDictionaryTransaction70; // 699 999 999 - 709 999 999 ~27.734 GB

        private Dictionary<long, string> _bigDictionaryTransaction71; // 709 999 999 - 719 999 999
        private Dictionary<long, string> _bigDictionaryTransaction72; // 719 999 999 - 729 999 999
        private Dictionary<long, string> _bigDictionaryTransaction73; // 729 999 999 - 739 999 999
        private Dictionary<long, string> _bigDictionaryTransaction74; // 739 999 999 - 749 999 999
        private Dictionary<long, string> _bigDictionaryTransaction75; // 749 999 999 - 759 999 999
        private Dictionary<long, string> _bigDictionaryTransaction76; // 759 999 999 - 769 999 999
        private Dictionary<long, string> _bigDictionaryTransaction77; // 769 999 999 - 779 999 999
        private Dictionary<long, string> _bigDictionaryTransaction78; // 779 999 999 - 789 999 999
        private Dictionary<long, string> _bigDictionaryTransaction79; // 789 999 999 - 799 999 999
        private Dictionary<long, string> _bigDictionaryTransaction80; // 799 999 999 - 809 999 999 ~31.696 GB

        private Dictionary<long, string> _bigDictionaryTransaction81; // 809 999 999 - 819 999 999
        private Dictionary<long, string> _bigDictionaryTransaction82; // 819 999 999 - 829 999 999
        private Dictionary<long, string> _bigDictionaryTransaction83; // 829 999 999 - 839 999 999
        private Dictionary<long, string> _bigDictionaryTransaction84; // 839 999 999 - 849 999 999
        private Dictionary<long, string> _bigDictionaryTransaction85; // 849 999 999 - 859 999 999
        private Dictionary<long, string> _bigDictionaryTransaction86; // 859 999 999 - 869 999 999
        private Dictionary<long, string> _bigDictionaryTransaction87; // 869 999 999 - 879 999 999
        private Dictionary<long, string> _bigDictionaryTransaction88; // 879 999 999 - 889 999 999
        private Dictionary<long, string> _bigDictionaryTransaction89; // 889 999 999 - 899 999 999
        private Dictionary<long, string> _bigDictionaryTransaction90; // 899 999 999 - 909 999 999 ~35.658 GB

        private Dictionary<long, string> _bigDictionaryTransaction91; // 909 999 999 - 919 999 999
        private Dictionary<long, string> _bigDictionaryTransaction92; // 919 999 999 - 929 999 999
        private Dictionary<long, string> _bigDictionaryTransaction93; // 929 999 999 - 939 999 999
        private Dictionary<long, string> _bigDictionaryTransaction94; // 939 999 999 - 949 999 999
        private Dictionary<long, string> _bigDictionaryTransaction95; // 949 999 999 - 959 999 999
        private Dictionary<long, string> _bigDictionaryTransaction96; // 959 999 999 - 969 999 999
        private Dictionary<long, string> _bigDictionaryTransaction97; // 969 999 999 - 979 999 999
        private Dictionary<long, string> _bigDictionaryTransaction98; // 979 999 999 - 989 999 999
        private Dictionary<long, string> _bigDictionaryTransaction99; // 989 999 999 - 999 999 999
        private Dictionary<long, string> _bigDictionaryTransaction100; // 999 999 999 - 1 009 999 999 ~39.620 GB


        public const int MaxTransactionPerDictionary = 100000000; // 100 millions of transactions per dictionary


        public ClassBigTransaction()
        {
            _bigDictionaryTransaction1 = new Dictionary<long, string>();
            _bigDictionaryTransaction2 = new Dictionary<long, string>();
            _bigDictionaryTransaction3 = new Dictionary<long, string>();
            _bigDictionaryTransaction4 = new Dictionary<long, string>();
            _bigDictionaryTransaction5 = new Dictionary<long, string>();
            _bigDictionaryTransaction6 = new Dictionary<long, string>();
            _bigDictionaryTransaction7 = new Dictionary<long, string>();
            _bigDictionaryTransaction8 = new Dictionary<long, string>();
            _bigDictionaryTransaction9 = new Dictionary<long, string>();
            _bigDictionaryTransaction10 = new Dictionary<long, string>();

            _bigDictionaryTransaction11 = new Dictionary<long, string>();
            _bigDictionaryTransaction12 = new Dictionary<long, string>();
            _bigDictionaryTransaction13 = new Dictionary<long, string>();
            _bigDictionaryTransaction14 = new Dictionary<long, string>();
            _bigDictionaryTransaction15 = new Dictionary<long, string>();
            _bigDictionaryTransaction16 = new Dictionary<long, string>();
            _bigDictionaryTransaction17 = new Dictionary<long, string>();
            _bigDictionaryTransaction18 = new Dictionary<long, string>();
            _bigDictionaryTransaction19 = new Dictionary<long, string>();
            _bigDictionaryTransaction20 = new Dictionary<long, string>();

            _bigDictionaryTransaction21 = new Dictionary<long, string>();
            _bigDictionaryTransaction22 = new Dictionary<long, string>();
            _bigDictionaryTransaction23 = new Dictionary<long, string>();
            _bigDictionaryTransaction24 = new Dictionary<long, string>();
            _bigDictionaryTransaction25 = new Dictionary<long, string>();
            _bigDictionaryTransaction26 = new Dictionary<long, string>();
            _bigDictionaryTransaction27 = new Dictionary<long, string>();
            _bigDictionaryTransaction28 = new Dictionary<long, string>();
            _bigDictionaryTransaction29 = new Dictionary<long, string>();
            _bigDictionaryTransaction30 = new Dictionary<long, string>();

            _bigDictionaryTransaction31 = new Dictionary<long, string>();
            _bigDictionaryTransaction32 = new Dictionary<long, string>();
            _bigDictionaryTransaction33 = new Dictionary<long, string>();
            _bigDictionaryTransaction34 = new Dictionary<long, string>();
            _bigDictionaryTransaction35 = new Dictionary<long, string>();
            _bigDictionaryTransaction36 = new Dictionary<long, string>();
            _bigDictionaryTransaction37 = new Dictionary<long, string>();
            _bigDictionaryTransaction38 = new Dictionary<long, string>();
            _bigDictionaryTransaction39 = new Dictionary<long, string>();
            _bigDictionaryTransaction40 = new Dictionary<long, string>();

            _bigDictionaryTransaction41 = new Dictionary<long, string>();
            _bigDictionaryTransaction42 = new Dictionary<long, string>();
            _bigDictionaryTransaction43 = new Dictionary<long, string>();
            _bigDictionaryTransaction44 = new Dictionary<long, string>();
            _bigDictionaryTransaction45 = new Dictionary<long, string>();
            _bigDictionaryTransaction46 = new Dictionary<long, string>();
            _bigDictionaryTransaction47 = new Dictionary<long, string>();
            _bigDictionaryTransaction48 = new Dictionary<long, string>();
            _bigDictionaryTransaction49 = new Dictionary<long, string>();
            _bigDictionaryTransaction50 = new Dictionary<long, string>();

            _bigDictionaryTransaction51 = new Dictionary<long, string>();
            _bigDictionaryTransaction52 = new Dictionary<long, string>();
            _bigDictionaryTransaction53 = new Dictionary<long, string>();
            _bigDictionaryTransaction54 = new Dictionary<long, string>();
            _bigDictionaryTransaction55 = new Dictionary<long, string>();
            _bigDictionaryTransaction56 = new Dictionary<long, string>();
            _bigDictionaryTransaction57 = new Dictionary<long, string>();
            _bigDictionaryTransaction58 = new Dictionary<long, string>();
            _bigDictionaryTransaction59 = new Dictionary<long, string>();
            _bigDictionaryTransaction60 = new Dictionary<long, string>();

            _bigDictionaryTransaction61 = new Dictionary<long, string>();
            _bigDictionaryTransaction62 = new Dictionary<long, string>();
            _bigDictionaryTransaction63 = new Dictionary<long, string>();
            _bigDictionaryTransaction64 = new Dictionary<long, string>();
            _bigDictionaryTransaction65 = new Dictionary<long, string>();
            _bigDictionaryTransaction66 = new Dictionary<long, string>();
            _bigDictionaryTransaction67 = new Dictionary<long, string>();
            _bigDictionaryTransaction68 = new Dictionary<long, string>();
            _bigDictionaryTransaction69 = new Dictionary<long, string>();
            _bigDictionaryTransaction70 = new Dictionary<long, string>();

            _bigDictionaryTransaction71 = new Dictionary<long, string>();
            _bigDictionaryTransaction72 = new Dictionary<long, string>();
            _bigDictionaryTransaction73 = new Dictionary<long, string>();
            _bigDictionaryTransaction74 = new Dictionary<long, string>();
            _bigDictionaryTransaction75 = new Dictionary<long, string>();
            _bigDictionaryTransaction76 = new Dictionary<long, string>();
            _bigDictionaryTransaction77 = new Dictionary<long, string>();
            _bigDictionaryTransaction78 = new Dictionary<long, string>();
            _bigDictionaryTransaction79 = new Dictionary<long, string>();
            _bigDictionaryTransaction80 = new Dictionary<long, string>();

            _bigDictionaryTransaction81 = new Dictionary<long, string>();
            _bigDictionaryTransaction82 = new Dictionary<long, string>();
            _bigDictionaryTransaction83 = new Dictionary<long, string>();
            _bigDictionaryTransaction84 = new Dictionary<long, string>();
            _bigDictionaryTransaction85 = new Dictionary<long, string>();
            _bigDictionaryTransaction86 = new Dictionary<long, string>();
            _bigDictionaryTransaction87 = new Dictionary<long, string>();
            _bigDictionaryTransaction88 = new Dictionary<long, string>();
            _bigDictionaryTransaction89 = new Dictionary<long, string>();
            _bigDictionaryTransaction90 = new Dictionary<long, string>();

            _bigDictionaryTransaction91 = new Dictionary<long, string>();
            _bigDictionaryTransaction92 = new Dictionary<long, string>();
            _bigDictionaryTransaction93 = new Dictionary<long, string>();
            _bigDictionaryTransaction94 = new Dictionary<long, string>();
            _bigDictionaryTransaction95 = new Dictionary<long, string>();
            _bigDictionaryTransaction96 = new Dictionary<long, string>();
            _bigDictionaryTransaction97 = new Dictionary<long, string>();
            _bigDictionaryTransaction98 = new Dictionary<long, string>();
            _bigDictionaryTransaction99 = new Dictionary<long, string>();
            _bigDictionaryTransaction100 = new Dictionary<long, string>();
        }

        public long Count
        {
            get
            {
                return CountTransaction();
            }
        }


        /// <summary>
        /// Insert transaction
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        public bool InsertTransaction(long id, string transaction)
        {
            try
            {
                long idDictionary = (long)Math.Ceiling((double)(id / MaxTransactionPerDictionary));
                if (idDictionary > 0)
                {
                    idDictionary -= 1;
                }
                switch (idDictionary)
                {
                    case 0:
                        _bigDictionaryTransaction1.Add(id, transaction);
                        break;
                    case 1:
                        _bigDictionaryTransaction2.Add(id, transaction);
                        break;
                    case 2:
                        _bigDictionaryTransaction3.Add(id, transaction);
                        break;
                    case 3:
                        _bigDictionaryTransaction4.Add(id, transaction);
                        break;
                    case 4:
                        _bigDictionaryTransaction5.Add(id, transaction);
                        break;
                    case 5:
                        _bigDictionaryTransaction6.Add(id, transaction);
                        break;
                    case 6:
                        _bigDictionaryTransaction7.Add(id, transaction);
                        break;
                    case 7:
                        _bigDictionaryTransaction8.Add(id, transaction);
                        break;
                    case 8:
                        _bigDictionaryTransaction9.Add(id, transaction);
                        break;
                    case 9:
                        _bigDictionaryTransaction10.Add(id, transaction);
                        break;
                    case 10:
                        _bigDictionaryTransaction11.Add(id, transaction);
                        break;
                    case 11:
                        _bigDictionaryTransaction12.Add(id, transaction);
                        break;
                    case 12:
                        _bigDictionaryTransaction13.Add(id, transaction);
                        break;
                    case 13:
                        _bigDictionaryTransaction14.Add(id, transaction);
                        break;
                    case 14:
                        _bigDictionaryTransaction15.Add(id, transaction);
                        break;
                    case 15:
                        _bigDictionaryTransaction16.Add(id, transaction);
                        break;
                    case 16:
                        _bigDictionaryTransaction17.Add(id, transaction);
                        break;
                    case 17:
                        _bigDictionaryTransaction18.Add(id, transaction);
                        break;
                    case 18:
                        _bigDictionaryTransaction19.Add(id, transaction);
                        break;
                    case 19:
                        _bigDictionaryTransaction20.Add(id, transaction);
                        break;
                    case 20:
                        _bigDictionaryTransaction21.Add(id, transaction);
                        break;
                    case 21:
                        _bigDictionaryTransaction22.Add(id, transaction);
                        break;
                    case 22:
                        _bigDictionaryTransaction23.Add(id, transaction);
                        break;
                    case 23:
                        _bigDictionaryTransaction24.Add(id, transaction);
                        break;
                    case 24:
                        _bigDictionaryTransaction25.Add(id, transaction);
                        break;
                    case 25:
                        _bigDictionaryTransaction26.Add(id, transaction);
                        break;
                    case 26:
                        _bigDictionaryTransaction27.Add(id, transaction);
                        break;
                    case 27:
                        _bigDictionaryTransaction28.Add(id, transaction);
                        break;
                    case 28:
                        _bigDictionaryTransaction29.Add(id, transaction);
                        break;
                    case 29:
                        _bigDictionaryTransaction30.Add(id, transaction);
                        break;
                    case 30:
                        _bigDictionaryTransaction31.Add(id, transaction);
                        break;
                    case 31:
                        _bigDictionaryTransaction32.Add(id, transaction);
                        break;
                    case 32:
                        _bigDictionaryTransaction33.Add(id, transaction);
                        break;
                    case 33:
                        _bigDictionaryTransaction34.Add(id, transaction);
                        break;
                    case 34:
                        _bigDictionaryTransaction35.Add(id, transaction);
                        break;
                    case 35:
                        _bigDictionaryTransaction36.Add(id, transaction);
                        break;
                    case 36:
                        _bigDictionaryTransaction37.Add(id, transaction);
                        break;
                    case 37:
                        _bigDictionaryTransaction38.Add(id, transaction);
                        break;
                    case 38:
                        _bigDictionaryTransaction39.Add(id, transaction);
                        break;
                    case 39:
                        _bigDictionaryTransaction40.Add(id, transaction);
                        break;
                    case 40:
                        _bigDictionaryTransaction41.Add(id, transaction);
                        break;
                    case 41:
                        _bigDictionaryTransaction42.Add(id, transaction);
                        break;
                    case 42:
                        _bigDictionaryTransaction43.Add(id, transaction);
                        break;
                    case 43:
                        _bigDictionaryTransaction44.Add(id, transaction);
                        break;
                    case 44:
                        _bigDictionaryTransaction45.Add(id, transaction);
                        break;
                    case 45:
                        _bigDictionaryTransaction46.Add(id, transaction);
                        break;
                    case 46:
                        _bigDictionaryTransaction47.Add(id, transaction);
                        break;
                    case 47:
                        _bigDictionaryTransaction48.Add(id, transaction);
                        break;
                    case 48:
                        _bigDictionaryTransaction49.Add(id, transaction);
                        break;
                    case 49:
                        _bigDictionaryTransaction50.Add(id, transaction);
                        break;
                    case 50:
                        _bigDictionaryTransaction51.Add(id, transaction);
                        break;
                    case 51:
                        _bigDictionaryTransaction52.Add(id, transaction);
                        break;
                    case 52:
                        _bigDictionaryTransaction53.Add(id, transaction);
                        break;
                    case 53:
                        _bigDictionaryTransaction54.Add(id, transaction);
                        break;
                    case 54:
                        _bigDictionaryTransaction55.Add(id, transaction);
                        break;
                    case 55:
                        _bigDictionaryTransaction56.Add(id, transaction);
                        break;
                    case 56:
                        _bigDictionaryTransaction57.Add(id, transaction);
                        break;
                    case 57:
                        _bigDictionaryTransaction58.Add(id, transaction);
                        break;
                    case 58:
                        _bigDictionaryTransaction59.Add(id, transaction);
                        break;
                    case 59:
                        _bigDictionaryTransaction60.Add(id, transaction);
                        break;
                    case 60:
                        _bigDictionaryTransaction61.Add(id, transaction);
                        break;
                    case 61:
                        _bigDictionaryTransaction62.Add(id, transaction);
                        break;
                    case 62:
                        _bigDictionaryTransaction63.Add(id, transaction);
                        break;
                    case 63:
                        _bigDictionaryTransaction64.Add(id, transaction);
                        break;
                    case 64:
                        _bigDictionaryTransaction65.Add(id, transaction);
                        break;
                    case 65:
                        _bigDictionaryTransaction66.Add(id, transaction);
                        break;
                    case 66:
                        _bigDictionaryTransaction67.Add(id, transaction);
                        break;
                    case 67:
                        _bigDictionaryTransaction68.Add(id, transaction);
                        break;
                    case 68:
                        _bigDictionaryTransaction69.Add(id, transaction);
                        break;
                    case 69:
                        _bigDictionaryTransaction70.Add(id, transaction);
                        break;
                    case 70:
                        _bigDictionaryTransaction71.Add(id, transaction);
                        break;
                    case 71:
                        _bigDictionaryTransaction72.Add(id, transaction);
                        break;
                    case 72:
                        _bigDictionaryTransaction73.Add(id, transaction);
                        break;
                    case 73:
                        _bigDictionaryTransaction74.Add(id, transaction);
                        break;
                    case 74:
                        _bigDictionaryTransaction75.Add(id, transaction);
                        break;
                    case 75:
                        _bigDictionaryTransaction76.Add(id, transaction);
                        break;
                    case 76:
                        _bigDictionaryTransaction77.Add(id, transaction);
                        break;
                    case 77:
                        _bigDictionaryTransaction78.Add(id, transaction);
                        break;
                    case 78:
                        _bigDictionaryTransaction79.Add(id, transaction);
                        break;
                    case 79:
                        _bigDictionaryTransaction80.Add(id, transaction);
                        break;
                    case 80:
                        _bigDictionaryTransaction81.Add(id, transaction);
                        break;
                    case 81:
                        _bigDictionaryTransaction82.Add(id, transaction);
                        break;
                    case 82:
                        _bigDictionaryTransaction83.Add(id, transaction);
                        break;
                    case 83:
                        _bigDictionaryTransaction84.Add(id, transaction);
                        break;
                    case 84:
                        _bigDictionaryTransaction85.Add(id, transaction);
                        break;
                    case 85:
                        _bigDictionaryTransaction86.Add(id, transaction);
                        break;
                    case 86:
                        _bigDictionaryTransaction87.Add(id, transaction);
                        break;
                    case 87:
                        _bigDictionaryTransaction88.Add(id, transaction);
                        break;
                    case 88:
                        _bigDictionaryTransaction89.Add(id, transaction);
                        break;
                    case 89:
                        _bigDictionaryTransaction90.Add(id, transaction);
                        break;
                    case 90:
                        _bigDictionaryTransaction91.Add(id, transaction);
                        break;
                    case 91:
                        _bigDictionaryTransaction92.Add(id, transaction);
                        break;
                    case 92:
                        _bigDictionaryTransaction93.Add(id, transaction);
                        break;
                    case 93:
                        _bigDictionaryTransaction94.Add(id, transaction);
                        break;
                    case 94:
                        _bigDictionaryTransaction95.Add(id, transaction);
                        break;
                    case 95:
                        _bigDictionaryTransaction96.Add(id, transaction);
                        break;
                    case 96:
                        _bigDictionaryTransaction97.Add(id, transaction);
                        break;
                    case 97:
                        _bigDictionaryTransaction98.Add(id, transaction);
                        break;
                    case 98:
                        _bigDictionaryTransaction99.Add(id, transaction);
                        break;
                    case 99:
                        _bigDictionaryTransaction100.Add(id, transaction);
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieve transaction information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTransaction(long id)
        {
            if (id < 0)
            {
                return "WRONG";
            }

            long idDictionary = (long)Math.Ceiling((double)(id / MaxTransactionPerDictionary));
            if (idDictionary > 0)
            {
                idDictionary -= 1;
            }
            switch (idDictionary)
            {
                case 0:
                    return _bigDictionaryTransaction1[id];
                case 1:
                    return _bigDictionaryTransaction2[id];
                case 2:
                    return _bigDictionaryTransaction3[id];
                case 3:
                    return _bigDictionaryTransaction4[id];
                case 4:
                    return _bigDictionaryTransaction5[id];
                case 5:
                    return _bigDictionaryTransaction6[id];
                case 6:
                    return _bigDictionaryTransaction7[id];
                case 7:
                    return _bigDictionaryTransaction8[id];
                case 8:
                    return _bigDictionaryTransaction9[id];
                case 9:
                    return _bigDictionaryTransaction10[id];
                case 10:
                    return _bigDictionaryTransaction11[id];
                case 11:
                    return _bigDictionaryTransaction12[id];
                case 12:
                    return _bigDictionaryTransaction13[id];
                case 13:
                    return _bigDictionaryTransaction14[id];
                case 14:
                    return _bigDictionaryTransaction15[id];
                case 15:
                    return _bigDictionaryTransaction16[id];
                case 16:
                    return _bigDictionaryTransaction17[id];
                case 17:
                    return _bigDictionaryTransaction18[id];
                case 18:
                    return _bigDictionaryTransaction19[id];
                case 19:
                    return _bigDictionaryTransaction20[id];
                case 20:
                    return _bigDictionaryTransaction21[id];
                case 21:
                    return _bigDictionaryTransaction22[id];
                case 22:
                    return _bigDictionaryTransaction23[id];
                case 23:
                    return _bigDictionaryTransaction24[id];
                case 24:
                    return _bigDictionaryTransaction25[id];
                case 25:
                    return _bigDictionaryTransaction26[id];
                case 26:
                    return _bigDictionaryTransaction27[id];
                case 27:
                    return _bigDictionaryTransaction28[id];
                case 28:
                    return _bigDictionaryTransaction29[id];
                case 29:
                    return _bigDictionaryTransaction30[id];
                case 30:
                    return _bigDictionaryTransaction31[id];
                case 31:
                    return _bigDictionaryTransaction32[id];
                case 32:
                    return _bigDictionaryTransaction33[id];
                case 33:
                    return _bigDictionaryTransaction34[id];
                case 34:
                    return _bigDictionaryTransaction35[id];
                case 35:
                    return _bigDictionaryTransaction36[id];
                case 36:
                    return _bigDictionaryTransaction37[id];
                case 37:
                    return _bigDictionaryTransaction38[id];
                case 38:
                    return _bigDictionaryTransaction39[id];
                case 39:
                    return _bigDictionaryTransaction40[id];
                case 40:
                    return _bigDictionaryTransaction41[id];
                case 41:
                    return _bigDictionaryTransaction42[id];
                case 42:
                    return _bigDictionaryTransaction43[id];
                case 43:
                    return _bigDictionaryTransaction44[id];
                case 44:
                    return _bigDictionaryTransaction45[id];
                case 45:
                    return _bigDictionaryTransaction46[id];
                case 46:
                    return _bigDictionaryTransaction47[id];
                case 47:
                    return _bigDictionaryTransaction48[id];
                case 48:
                    return _bigDictionaryTransaction49[id];
                case 49:
                    return _bigDictionaryTransaction50[id];
                case 50:
                    return _bigDictionaryTransaction51[id];
                case 51:
                    return _bigDictionaryTransaction52[id];
                case 52:
                    return _bigDictionaryTransaction53[id];
                case 53:
                    return _bigDictionaryTransaction54[id];
                case 54:
                    return _bigDictionaryTransaction55[id];
                case 55:
                    return _bigDictionaryTransaction56[id];
                case 56:
                    return _bigDictionaryTransaction57[id];
                case 57:
                    return _bigDictionaryTransaction58[id];
                case 58:
                    return _bigDictionaryTransaction59[id];
                case 59:
                    return _bigDictionaryTransaction60[id];
                case 60:
                    return _bigDictionaryTransaction61[id];
                case 61:
                    return _bigDictionaryTransaction62[id];
                case 62:
                    return _bigDictionaryTransaction63[id];
                case 63:
                    return _bigDictionaryTransaction64[id];
                case 64:
                    return _bigDictionaryTransaction65[id];
                case 65:
                    return _bigDictionaryTransaction66[id];
                case 66:
                    return _bigDictionaryTransaction67[id];
                case 67:
                    return _bigDictionaryTransaction68[id];
                case 68:
                    return _bigDictionaryTransaction69[id];
                case 69:
                    return _bigDictionaryTransaction70[id];
                case 70:
                    return _bigDictionaryTransaction71[id];
                case 71:
                    return _bigDictionaryTransaction72[id];
                case 72:
                    return _bigDictionaryTransaction73[id];
                case 73:
                    return _bigDictionaryTransaction74[id];
                case 74:
                    return _bigDictionaryTransaction75[id];
                case 75:
                    return _bigDictionaryTransaction76[id];
                case 76:
                    return _bigDictionaryTransaction77[id];
                case 77:
                    return _bigDictionaryTransaction78[id];
                case 78:
                    return _bigDictionaryTransaction79[id];
                case 79:
                    return _bigDictionaryTransaction80[id];
                case 80:
                    return _bigDictionaryTransaction81[id];
                case 81:
                    return _bigDictionaryTransaction82[id];
                case 82:
                    return _bigDictionaryTransaction83[id];
                case 83:
                    return _bigDictionaryTransaction84[id];
                case 84:
                    return _bigDictionaryTransaction85[id];
                case 85:
                    return _bigDictionaryTransaction86[id];
                case 86:
                    return _bigDictionaryTransaction87[id];
                case 87:
                    return _bigDictionaryTransaction88[id];
                case 88:
                    return _bigDictionaryTransaction89[id];
                case 89:
                    return _bigDictionaryTransaction90[id];
                case 90:
                    return _bigDictionaryTransaction91[id];
                case 91:
                    return _bigDictionaryTransaction92[id];
                case 92:
                    return _bigDictionaryTransaction93[id];
                case 93:
                    return _bigDictionaryTransaction94[id];
                case 94:
                    return _bigDictionaryTransaction95[id];
                case 95:
                    return _bigDictionaryTransaction96[id];
                case 96:
                    return _bigDictionaryTransaction97[id];
                case 97:
                    return _bigDictionaryTransaction98[id];
                case 98:
                    return _bigDictionaryTransaction99[id];
                case 99:
                    return _bigDictionaryTransaction100[id];
            }
            return "WRONG";
        }



        /// <summary>
        /// Retrieve total transaction saved.
        /// </summary>
        /// <returns></returns>
        public long CountTransaction()
        {
            return _bigDictionaryTransaction1.Count +
                _bigDictionaryTransaction2.Count +
                _bigDictionaryTransaction3.Count +
                _bigDictionaryTransaction4.Count +
                _bigDictionaryTransaction5.Count +
                _bigDictionaryTransaction6.Count +
                _bigDictionaryTransaction7.Count +
                _bigDictionaryTransaction8.Count +
                _bigDictionaryTransaction9.Count +
                _bigDictionaryTransaction10.Count +
                _bigDictionaryTransaction11.Count +
                _bigDictionaryTransaction12.Count +
                _bigDictionaryTransaction13.Count +
                _bigDictionaryTransaction14.Count +
                _bigDictionaryTransaction15.Count +
                _bigDictionaryTransaction16.Count +
                _bigDictionaryTransaction17.Count +
                _bigDictionaryTransaction18.Count +
                _bigDictionaryTransaction19.Count +
                _bigDictionaryTransaction20.Count +
                _bigDictionaryTransaction21.Count +
                _bigDictionaryTransaction22.Count +
                _bigDictionaryTransaction23.Count +
                _bigDictionaryTransaction24.Count +
                _bigDictionaryTransaction25.Count +
                _bigDictionaryTransaction26.Count +
                _bigDictionaryTransaction27.Count +
                _bigDictionaryTransaction28.Count +
                _bigDictionaryTransaction29.Count +
                _bigDictionaryTransaction30.Count +
                _bigDictionaryTransaction31.Count +
                _bigDictionaryTransaction32.Count +
                _bigDictionaryTransaction33.Count +
                _bigDictionaryTransaction34.Count +
                _bigDictionaryTransaction35.Count +
                _bigDictionaryTransaction36.Count +
                _bigDictionaryTransaction37.Count +
                _bigDictionaryTransaction38.Count +
                _bigDictionaryTransaction39.Count +
                _bigDictionaryTransaction40.Count +
                _bigDictionaryTransaction41.Count +
                _bigDictionaryTransaction42.Count +
                _bigDictionaryTransaction43.Count +
                _bigDictionaryTransaction44.Count +
                _bigDictionaryTransaction45.Count +
                _bigDictionaryTransaction46.Count +
                _bigDictionaryTransaction47.Count +
                _bigDictionaryTransaction48.Count +
                _bigDictionaryTransaction49.Count +
                _bigDictionaryTransaction50.Count +
                _bigDictionaryTransaction51.Count +
                _bigDictionaryTransaction52.Count +
                _bigDictionaryTransaction53.Count +
                _bigDictionaryTransaction54.Count +
                _bigDictionaryTransaction55.Count +
                _bigDictionaryTransaction56.Count +
                _bigDictionaryTransaction57.Count +
                _bigDictionaryTransaction58.Count +
                _bigDictionaryTransaction59.Count +
                _bigDictionaryTransaction60.Count +
                _bigDictionaryTransaction61.Count +
                _bigDictionaryTransaction62.Count +
                _bigDictionaryTransaction63.Count +
                _bigDictionaryTransaction64.Count +
                _bigDictionaryTransaction65.Count +
                _bigDictionaryTransaction66.Count +
                _bigDictionaryTransaction67.Count +
                _bigDictionaryTransaction68.Count +
                _bigDictionaryTransaction69.Count +
                _bigDictionaryTransaction70.Count +
                _bigDictionaryTransaction71.Count +
                _bigDictionaryTransaction72.Count +
                _bigDictionaryTransaction73.Count +
                _bigDictionaryTransaction74.Count +
                _bigDictionaryTransaction75.Count +
                _bigDictionaryTransaction76.Count +
                _bigDictionaryTransaction77.Count +
                _bigDictionaryTransaction78.Count +
                _bigDictionaryTransaction79.Count +
                _bigDictionaryTransaction80.Count +
                _bigDictionaryTransaction81.Count +
                _bigDictionaryTransaction82.Count +
                _bigDictionaryTransaction83.Count +
                _bigDictionaryTransaction84.Count +
                _bigDictionaryTransaction85.Count +
                _bigDictionaryTransaction86.Count +
                _bigDictionaryTransaction87.Count +
                _bigDictionaryTransaction88.Count +
                _bigDictionaryTransaction89.Count +
                _bigDictionaryTransaction90.Count +
                _bigDictionaryTransaction91.Count +
                _bigDictionaryTransaction92.Count +
                _bigDictionaryTransaction93.Count +
                _bigDictionaryTransaction94.Count +
                _bigDictionaryTransaction95.Count +
                _bigDictionaryTransaction96.Count +
                _bigDictionaryTransaction97.Count +
                _bigDictionaryTransaction98.Count +
                _bigDictionaryTransaction99.Count +
                _bigDictionaryTransaction100.Count;
        }

        /// <summary>
        /// Check on every dictionary the id existance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsKey(long id)
        {
            if (_bigDictionaryTransaction1.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction2.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction3.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction4.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction5.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction6.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction7.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction8.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction9.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction10.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction11.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction12.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction13.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction14.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction15.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction16.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction17.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction18.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction19.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction20.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction21.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction22.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction23.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction24.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction25.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction26.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction27.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction28.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction29.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction30.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction31.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction32.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction33.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction34.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction35.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction36.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction37.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction38.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction39.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction40.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction41.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction42.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction43.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction44.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction45.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction46.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction47.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction48.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction49.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction50.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction51.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction52.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction53.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction54.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction55.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction56.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction57.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction58.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction59.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction60.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction61.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction62.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction63.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction64.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction65.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction66.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction67.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction68.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction69.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction70.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction71.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction72.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction73.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction74.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction75.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction76.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction77.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction78.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction79.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction80.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction81.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction82.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction83.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction84.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction85.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction86.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction87.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction88.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction89.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction90.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction91.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction92.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction93.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction94.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction95.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction96.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction97.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction98.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction99.ContainsKey(id))
            {
                return true;
            }
            if (_bigDictionaryTransaction100.ContainsKey(id))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clear dictionnary
        /// </summary>
        public void Clear()
        {
            _bigDictionaryTransaction1.Clear();
            _bigDictionaryTransaction2.Clear();
            _bigDictionaryTransaction3.Clear();
            _bigDictionaryTransaction4.Clear();
            _bigDictionaryTransaction5.Clear();
            _bigDictionaryTransaction6.Clear();
            _bigDictionaryTransaction7.Clear();
            _bigDictionaryTransaction8.Clear();
            _bigDictionaryTransaction9.Clear();
            _bigDictionaryTransaction10.Clear();
            _bigDictionaryTransaction11.Clear();
            _bigDictionaryTransaction12.Clear();
            _bigDictionaryTransaction13.Clear();
            _bigDictionaryTransaction14.Clear();
            _bigDictionaryTransaction15.Clear();
            _bigDictionaryTransaction16.Clear();
            _bigDictionaryTransaction17.Clear();
            _bigDictionaryTransaction18.Clear();
            _bigDictionaryTransaction19.Clear();
            _bigDictionaryTransaction20.Clear();
            _bigDictionaryTransaction21.Clear();
            _bigDictionaryTransaction22.Clear();
            _bigDictionaryTransaction23.Clear();
            _bigDictionaryTransaction24.Clear();
            _bigDictionaryTransaction25.Clear();
            _bigDictionaryTransaction26.Clear();
            _bigDictionaryTransaction27.Clear();
            _bigDictionaryTransaction28.Clear();
            _bigDictionaryTransaction29.Clear();
            _bigDictionaryTransaction30.Clear();
            _bigDictionaryTransaction31.Clear();
            _bigDictionaryTransaction32.Clear();
            _bigDictionaryTransaction33.Clear();
            _bigDictionaryTransaction34.Clear();
            _bigDictionaryTransaction35.Clear();
            _bigDictionaryTransaction36.Clear();
            _bigDictionaryTransaction37.Clear();
            _bigDictionaryTransaction38.Clear();
            _bigDictionaryTransaction39.Clear();
            _bigDictionaryTransaction40.Clear();
            _bigDictionaryTransaction41.Clear();
            _bigDictionaryTransaction42.Clear();
            _bigDictionaryTransaction43.Clear();
            _bigDictionaryTransaction44.Clear();
            _bigDictionaryTransaction45.Clear();
            _bigDictionaryTransaction46.Clear();
            _bigDictionaryTransaction47.Clear();
            _bigDictionaryTransaction48.Clear();
            _bigDictionaryTransaction49.Clear();
            _bigDictionaryTransaction50.Clear();
            _bigDictionaryTransaction51.Clear();
            _bigDictionaryTransaction52.Clear();
            _bigDictionaryTransaction53.Clear();
            _bigDictionaryTransaction54.Clear();
            _bigDictionaryTransaction55.Clear();
            _bigDictionaryTransaction56.Clear();
            _bigDictionaryTransaction57.Clear();
            _bigDictionaryTransaction58.Clear();
            _bigDictionaryTransaction59.Clear();
            _bigDictionaryTransaction60.Clear();
            _bigDictionaryTransaction61.Clear();
            _bigDictionaryTransaction62.Clear();
            _bigDictionaryTransaction63.Clear();
            _bigDictionaryTransaction64.Clear();
            _bigDictionaryTransaction65.Clear();
            _bigDictionaryTransaction66.Clear();
            _bigDictionaryTransaction67.Clear();
            _bigDictionaryTransaction68.Clear();
            _bigDictionaryTransaction69.Clear();
            _bigDictionaryTransaction70.Clear();
            _bigDictionaryTransaction71.Clear();
            _bigDictionaryTransaction72.Clear();
            _bigDictionaryTransaction73.Clear();
            _bigDictionaryTransaction74.Clear();
            _bigDictionaryTransaction75.Clear();
            _bigDictionaryTransaction76.Clear();
            _bigDictionaryTransaction77.Clear();
            _bigDictionaryTransaction78.Clear();
            _bigDictionaryTransaction79.Clear();
            _bigDictionaryTransaction80.Clear();
            _bigDictionaryTransaction81.Clear();
            _bigDictionaryTransaction82.Clear();
            _bigDictionaryTransaction83.Clear();
            _bigDictionaryTransaction84.Clear();
            _bigDictionaryTransaction85.Clear();
            _bigDictionaryTransaction86.Clear();
            _bigDictionaryTransaction87.Clear();
            _bigDictionaryTransaction88.Clear();
            _bigDictionaryTransaction89.Clear();
            _bigDictionaryTransaction90.Clear();
            _bigDictionaryTransaction91.Clear();
            _bigDictionaryTransaction92.Clear();
            _bigDictionaryTransaction93.Clear();
            _bigDictionaryTransaction94.Clear();
            _bigDictionaryTransaction95.Clear();
            _bigDictionaryTransaction96.Clear();
            _bigDictionaryTransaction97.Clear();
            _bigDictionaryTransaction98.Clear();
            _bigDictionaryTransaction99.Clear();
            _bigDictionaryTransaction100.Clear();
        }
    }
}
