// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MeisterGeister.Logic.Voraussetzungen
{
    #region ParseTree
    [Serializable]
    public class ParseErrors : List<ParseError>
    {
    }

    [Serializable]
    public class ParseError
    {
        private string message;
        private int code;
        private int line;
        private int col;
        private int pos;
        private int length;

        public int Code { get { return code; } }
        public int Line { get { return line; } }
        public int Column { get { return col; } }
        public int Position { get { return pos; } }
        public int Length { get { return length; } }
        public string Message { get { return message; } }

        // just for the sake of serialization
        public ParseError()
        {
        }

        public ParseError(string message, int code, ParseNode node) : this(message, code,  0, node.Token.StartPos, node.Token.StartPos, node.Token.Length)
        {
        }

        public ParseError(string message, int code, int line, int col, int pos, int length)
        {
            this.message = message;
            this.code = code;
            this.line = line;
            this.col = col;
            this.pos = pos;
            this.length = length;
        }
    }

    // rootlevel of the node tree
    [Serializable]
    public partial class ParseTree : ParseNode
    {
        public ParseErrors Errors;

        public List<Token> Skipped;

        public ParseTree() : base(new Token(), "ParseTree")
        {
            Token.Type = TokenType.Start;
            Token.Text = "Root";
            Errors = new ParseErrors();
        }

        public string PrintTree()
        {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            PrintNode(sb, this, indent);
            return sb.ToString();
        }

        private void PrintNode(StringBuilder sb, ParseNode node, int indent)
        {
            
            string space = "".PadLeft(indent, ' ');

            sb.Append(space);
            sb.AppendLine(node.Text);

            foreach (ParseNode n in node.Nodes)
                PrintNode(sb, n, indent + 2);
        }
        
        /// <summary>
        /// this is the entry point for executing and evaluating the parse tree.
        /// </summary>
        /// <param name="paramlist">additional optional input parameters</param>
        /// <returns>the output of the evaluation function</returns>
        public object Eval(params object[] paramlist)
        {
            return Nodes[0].Eval(this, paramlist);
        }
    }

    [Serializable]
    [XmlInclude(typeof(ParseTree))]
    public partial class ParseNode
    {
        protected string text;
        protected List<ParseNode> nodes;
        
        public List<ParseNode> Nodes { get {return nodes;} }
        
        [XmlIgnore] // avoid circular references when serializing
        public ParseNode Parent;
        public Token Token; // the token/rule

        [XmlIgnore] // skip redundant text (is part of Token)
        public string Text { // text to display in parse tree 
            get { return text;} 
            set { text = value; }
        } 

        public virtual ParseNode CreateNode(Token token, string text)
        {
            ParseNode node = new ParseNode(token, text);
            node.Parent = this;
            return node;
        }

        protected ParseNode(Token token, string text)
        {
            this.Token = token;
            this.text = text;
            this.nodes = new List<ParseNode>();
        }

        protected object GetValue(ParseTree tree, TokenType type, int index, params object[] paramlist)
        {
            return GetValue(tree, type, ref index, paramlist);
        }

        protected object GetValue(ParseTree tree, TokenType type, ref int index, params object[] paramlist)
        {
            object o = null;
            if (index < 0) return o;

            // left to right
            foreach (ParseNode node in nodes)
            {
                if (node.Token.Type == type)
                {
                    index--;
                    if (index < 0)
                    {
                        o = node.Eval(tree, paramlist);
                        break;
                    }
                }
            }
            return o;
        }

        /// <summary>
        /// this implements the evaluation functionality, cannot be used directly
        /// </summary>
        /// <param name="tree">the parsetree itself</param>
        /// <param name="paramlist">optional input parameters</param>
        /// <returns>a partial result of the evaluation</returns>
        internal object Eval(ParseTree tree, params object[] paramlist)
        {
            object Value = null;

            switch (Token.Type)
            {
                case TokenType.Start:
                    Value = EvalStart(tree, paramlist);
                    break;
                case TokenType.AndExpression:
                    Value = EvalAndExpression(tree, paramlist);
                    break;
                case TokenType.OrExpression:
                    Value = EvalOrExpression(tree, paramlist);
                    break;
                case TokenType.Atom:
                    Value = EvalAtom(tree, paramlist);
                    break;
                case TokenType.Text:
                    Value = EvalText(tree, paramlist);
                    break;
                case TokenType.Wert:
                    Value = EvalWert(tree, paramlist);
                    break;
                case TokenType.Eigenschaft:
                    Value = EvalEigenschaft(tree, paramlist);
                    break;
                case TokenType.Sonderfertigkeit:
                    Value = EvalSonderfertigkeit(tree, paramlist);
                    break;
                case TokenType.Vorteil:
                    Value = EvalVorteil(tree, paramlist);
                    break;
                case TokenType.Nachteil:
                    Value = EvalNachteil(tree, paramlist);
                    break;
                case TokenType.TalentZauber:
                    Value = EvalTalentZauber(tree, paramlist);
                    break;

                default:
                    Value = Token.Text;
                    break;
            }
            return Value;
        }

        protected virtual object EvalStart(ParseTree tree, params object[] paramlist)
        {
            if(this.GetValue(tree, TokenType.AndExpression, 0, paramlist) == null)
        		return true;
        	return this.GetValue(tree, TokenType.AndExpression, 0, paramlist);
        }

        protected virtual object EvalAndExpression(ParseTree tree, params object[] paramlist)
        {
            bool Value = Convert.ToBoolean(this.GetValue(tree, TokenType.OrExpression, 0, paramlist)); 
        	int i = 1;
        	while (Value && this.GetValue(tree, TokenType.OrExpression, i, paramlist) != null) { 
        		Value = Value && Convert.ToBoolean(this.GetValue(tree, TokenType.OrExpression, i++, paramlist)); 
        	} 
        	return Value;
        }

        protected virtual object EvalOrExpression(ParseTree tree, params object[] paramlist)
        {
            bool Value = Convert.ToBoolean(this.GetValue(tree, TokenType.Atom, 0, paramlist)); 
        	int i = 1;
        	while (!Value && this.GetValue(tree, TokenType.Atom, i, paramlist) != null) { 
        		Value = Value || Convert.ToBoolean(this.GetValue(tree, TokenType.Atom, i++, paramlist)); 
        	} 
        	return Value;
        }

        protected virtual object EvalAtom(ParseTree tree, params object[] paramlist)
        {
            bool atom;
        	if(this.GetValue(tree, TokenType.Sonderfertigkeit, 0, paramlist) != null)
        		atom = Convert.ToBoolean(this.GetValue(tree, TokenType.Sonderfertigkeit, 0, paramlist));
        	else if(this.GetValue(tree, TokenType.Vorteil, 0, paramlist) != null)
        		atom = Convert.ToBoolean(this.GetValue(tree, TokenType.Vorteil, 0, paramlist));
        	else if(this.GetValue(tree, TokenType.Nachteil, 0, paramlist)!= null)
        		atom = Convert.ToBoolean(this.GetValue(tree, TokenType.Nachteil, 0, paramlist));
        	else if(this.GetValue(tree, TokenType.Eigenschaft, 0, paramlist) != null)
        		atom = Convert.ToBoolean(this.GetValue(tree, TokenType.Eigenschaft, 0, paramlist));
        	else if(this.GetValue(tree, TokenType.TalentZauber, 0, paramlist) != null)
        		atom = Convert.ToBoolean(this.GetValue(tree, TokenType.TalentZauber, 0, paramlist));
        	else
        		atom = Convert.ToBoolean(this.GetValue(tree, TokenType.AndExpression, 0, paramlist));
        	if(this.GetValue(tree, TokenType.NOT, 0, paramlist) != null)
        		atom = !atom;
        	return atom;
        }

        protected virtual object EvalText(ParseTree tree, params object[] paramlist)
        {
            string Value = Convert.ToString(this.GetValue(tree, TokenType.WORT, 0, paramlist));
        	if(this.GetValue(tree, TokenType.BROPEN, 0, paramlist) != null)
        		Value = Value + " " + Convert.ToString(this.GetValue(tree, TokenType.BROPEN, 0, paramlist)) + Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist)) + Convert.ToString(this.GetValue(tree, TokenType.BRCLOSE, 0, paramlist)); 
        	else if(this.GetValue(tree, TokenType.Text, 0, paramlist) != null)
        		Value = Value + " " + Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist)); 
        	return Value;
        }

        protected virtual object EvalWert(ParseTree tree, params object[] paramlist)
        {
            return this.GetValue(tree, TokenType.Text, 0, paramlist);
        }

        protected virtual object EvalEigenschaft(ParseTree tree, params object[] paramlist)
        {
            //	return true;
        
        	MeisterGeister.Model.Held held = null;
        	if(paramlist != null && paramlist.Length > 0  && paramlist[0] is MeisterGeister.Model.Held) 
        		held = (MeisterGeister.Model.Held)paramlist[0];
        	if(held == null)
        		return false;
        	int wert = Convert.ToInt32(this.GetValue(tree, TokenType.NUMBER, 0, paramlist));
        	if(this.GetValue(tree, TokenType.KL, 0, paramlist) != null)
        		return held.KL >= wert;
        	if(this.GetValue(tree, TokenType.IN, 0, paramlist) != null)
        		return held.IN >= wert;
        	if(this.GetValue(tree, TokenType.CH, 0, paramlist) != null)
        		return held.CH >= wert;
        	if(this.GetValue(tree, TokenType.MU, 0, paramlist) != null)
        		return held.MU >= wert;
        	if(this.GetValue(tree, TokenType.KK, 0, paramlist) != null)
        		return held.KK >= wert;
        	if(this.GetValue(tree, TokenType.KO, 0, paramlist) != null)
        		return held.KO >= wert;
        	if(this.GetValue(tree, TokenType.GE, 0, paramlist) != null)
        		return held.GE >= wert;
        	if(this.GetValue(tree, TokenType.FF, 0, paramlist) != null)
        		return held.FF >= wert;
        	if(this.GetValue(tree, TokenType.AT, 0, paramlist) != null)
        		return held.AttackeBasis >= wert;
        	if(this.GetValue(tree, TokenType.PA, 0, paramlist) != null)
        		return held.ParadeBasis >= wert;
        	if(this.GetValue(tree, TokenType.FK, 0, paramlist) != null)
        		return held.FernkampfBasis >= wert;
        	if(this.GetValue(tree, TokenType.INI, 0, paramlist) != null)
        		return held.InitiativeBasisOhneSonderfertigkeiten >= wert;
        	return held.ParadeBasis >= wert;
        }

        protected virtual object EvalSonderfertigkeit(ParseTree tree, params object[] paramlist)
        {
            /*	if(Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist)).Equals("Elementarharmonisierte Aura (Fels/Luft)"))
        		return false;
        	if(Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist)).Equals("Kraftlinienmagie II"))
        		return true;
        //	throw new ArgumentException("y" + Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist)) + "X");
        	if(Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist)).Equals("Merkmalskenntnis (Dämonisch (Tasfarelel))"))
        		return true;
        	return false;
        */
        	MeisterGeister.Model.Held held = null;
        	if(paramlist != null && paramlist.Length > 0  && paramlist[0] is MeisterGeister.Model.Held) 
        		held = (MeisterGeister.Model.Held)paramlist[0];
        	if(held == null)
        		return false;
        	string sfName = Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist));
        	string wert = null;
        	if(this.GetValue(tree, TokenType.Wert, 0, paramlist) != null)
        		wert = Convert.ToString(this.GetValue(tree, TokenType.Wert, 0, paramlist));
        	return held.HatSonderfertigkeitUndVoraussetzungen(sfName, wert);
        }

        protected virtual object EvalVorteil(ParseTree tree, params object[] paramlist)
        {
            MeisterGeister.Model.Held held = null;
        	if(paramlist != null && paramlist.Length > 0  && paramlist[0] is MeisterGeister.Model.Held) 
        		held = (MeisterGeister.Model.Held)paramlist[0];
        	if(held == null)
        		return false;
        	string vName = Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist));
        	string wert = null;
        	if(this.GetValue(tree, TokenType.Wert, 0, paramlist) != null)
        		wert = Convert.ToString(this.GetValue(tree, TokenType.Wert, 0, paramlist));
        	return held.HatVorNachteil(vName, wert);
        }

        protected virtual object EvalNachteil(ParseTree tree, params object[] paramlist)
        {
            MeisterGeister.Model.Held held = null;
        	if(paramlist != null && paramlist.Length > 0  && paramlist[0] is MeisterGeister.Model.Held) 
        		held = (MeisterGeister.Model.Held)paramlist[0];
        	if(held == null)
        		return false;
        	string vName = Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist));
        	string wert = null;
        	if(this.GetValue(tree, TokenType.Wert, 0, paramlist) != null)
        		wert = Convert.ToString(this.GetValue(tree, TokenType.Wert, 0, paramlist));
        	return held.HatVorNachteil(vName, wert);
        }

        protected virtual object EvalTalentZauber(ParseTree tree, params object[] paramlist)
        {
            //	return true;
        
        	MeisterGeister.Model.Held held = null;
        	if(paramlist != null && paramlist.Length > 0  && paramlist[0] is MeisterGeister.Model.Held) 
        		held = (MeisterGeister.Model.Held)paramlist[0];
        	if(held == null)
        		return false;
        	string tName = Convert.ToString(this.GetValue(tree, TokenType.Text, 0, paramlist));
        	int wert = Convert.ToInt32(this.GetValue(tree, TokenType.NUMBER, 0, paramlist));
        	if (tName.ToUpperInvariant().Equals(tName))
        		return held.HatZauber(tName, wert);
        	return held.HatTalent(tName, wert);
        }


    }
    
    #endregion ParseTree
}
