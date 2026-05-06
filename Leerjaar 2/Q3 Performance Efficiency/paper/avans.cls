\NeedsTeXFormat{LaTeX2e}
\ProvidesClass{avans}[2026/03/24 v2.0 Avans-inspired report class]

\newif\if@dutch
\newif\if@english
\newif\if@print

\DeclareOption{dutch}{\@dutchtrue\@englishfalse}
\DeclareOption{english}{\@dutchfalse\@englishtrue}
\DeclareOption{print}{\@printtrue}
\DeclareOption*{\PassOptionsToClass{\CurrentOption}{article}}
\ExecuteOptions{english}
\ProcessOptions\relax

\LoadClass[12pt,a4paper]{article}

% Core packages
\RequirePackage{ifthen}
\RequirePackage{iftex}
\RequirePackage[T1]{fontenc}
\RequirePackage[utf8]{inputenc}
\if@dutch
  \RequirePackage[dutch]{babel}
\else
  \RequirePackage[dutch,english]{babel}
\fi
\RequirePackage{helvet}
\RequirePackage{tgpagella}
\RequirePackage{microtype}
\RequirePackage{geometry}
\RequirePackage{xcolor}
\RequirePackage{graphicx}
\RequirePackage{fancyhdr}
\RequirePackage[noindentafter]{titlesec}
\RequirePackage{titletoc}
\RequirePackage{setspace}
\RequirePackage{caption}
\RequirePackage{float}
\RequirePackage{booktabs}
\RequirePackage{tabularx}
\RequirePackage{array}
\RequirePackage{ragged2e}
\RequirePackage{tikz}
\RequirePackage{etoolbox}
\RequirePackage[numbers]{natbib}
\RequirePackage{csquotes}
\PassOptionsToPackage{hyphens}{url}
\RequirePackage{xurl}
\RequirePackage[plainpages=false]{hyperref}

% Brand palette (Avans-inspired)
\definecolor{avans-red}{HTML}{E11F26}
\definecolor{avans-black}{HTML}{111111}
\definecolor{avans-white}{HTML}{FFFFFF}
\definecolor{avans-gray}{HTML}{6B6B6B}
\definecolor{avans-light}{HTML}{F5F5F5}

\if@print
  \colorlet{accent}{black}
\else
  \colorlet{accent}{avans-red}
\fi

% Page and paragraph layout
\geometry{a4paper,left=30mm,right=30mm,top=28mm,bottom=30mm}
\setlength{\parindent}{0pt}
\setlength{\parskip}{0.6em}
\setstretch{1.12}

% Hyperref appearance
\hypersetup{
  colorlinks=true,
  citecolor=accent,
  linkcolor=accent,
  urlcolor=accent
}

% Typography defaults: sans text with serif headings
\renewcommand*\familydefault{\sfdefault}
\newcommand*\avansheadingfont{\rmfamily\bfseries}

% Metadata fields
\newcommand*\@programme{}
\newcommand*\@supervisor{}
\newcommand*\@institution{}
\newcommand*\@keywords{}
\newcommand*\@subtitle{}

\newcommand*\programme[1]{\gdef\@programme{#1}}
\newcommand*\supervisor[1]{\gdef\@supervisor{#1}}
\newcommand*\institution[1]{\gdef\@institution{#1}}
\newcommand*\subtitle[1]{\gdef\@subtitle{#1}}
\newcommand*\keywords[1]{%
  \gdef\@keywords{#1}%
  \par\smallskip\noindent\textbf{Keywords:} #1%
}

% Table helper used in content files
\newcommand*\tablehead[1]{\textbf{#1}}

% Section styling
\titleformat{\section}
  {\Large\avansheadingfont}
  {\color{accent}\thesection}
  {0.7em}
  {\color{avans-black}}
  [\vspace{0.2em}\color{accent}\titlerule]

\titleformat{\subsection}
  {\large\avansheadingfont}
  {\color{accent}\thesubsection}
  {0.6em}
  {\color{avans-black}}

\titleformat{\subsubsection}
  {\normalsize\avansheadingfont}
  {\color{accent}\thesubsubsection}
  {0.5em}
  {\color{avans-black}}

\titlespacing*{\section}{0pt}{1.5\baselineskip}{0.5\baselineskip}
\titlespacing*{\subsection}{0pt}{1.2\baselineskip}{0.3\baselineskip}
\titlespacing*{\subsubsection}{0pt}{0.9\baselineskip}{0.2\baselineskip}

% TOC styling
\renewcommand{\contentsname}{Table of Contents}
\dottedcontents{section}[0em]{\avansheadingfont}{2.2em}{0.8pc}
\dottedcontents{subsection}[2.8em]{}{2.6em}{0.8pc}

% Header/footer styling
\setlength{\headheight}{25pt}
\pagestyle{fancy}
\fancyhf{}
\fancyhead[L]{\small\color{avans-gray}\nouppercase{\leftmark}}
\fancyhead[R]{\small\avansheadingfont\thepage}
\renewcommand*\headrulewidth{0.4pt}
\renewcommand*\headrule{\hbox to\headwidth{\color{accent}\leaders\hrule height \headrulewidth\hfill}}
\renewcommand*\footrulewidth{0pt}

\renewcommand*\sectionmark[1]{\markboth{\thesection\enspace #1}{}}

\newcommand*\setheader[1]{\markboth{#1}{#1}}

% Title page - refined elegant design
\renewcommand*\maketitle{%
  \begin{titlepage}
    \hypersetup{pageanchor=false}
    \newgeometry{left=0mm,right=0mm,top=0mm,bottom=0mm}
    \thispagestyle{empty}
    \pagecolor{avans-white}
    
    \usetikzlibrary{calc}
    \begin{tikzpicture}[remember picture,overlay]
      % Clean white background
      \fill[avans-white] (current page.south west) rectangle (current page.north east);
      
      % Thin red line at top - institutional marker
      \draw[line width=3pt,color=accent] (current page.north west) -- (current page.north east);
      
      % Red accent block: left side, from top third to middle
      \fill[accent] (current page.west) rectangle ([xshift=8mm,yshift=-0.35\paperheight]current page.north west);
      
      % "AVANS" text rotated in left accent block
      \node[anchor=center,text=avans-white,rotate=90,
            font=\rmfamily\bfseries\fontsize{48}{56}\selectfont,opacity=0.15]
        at ([xshift=4mm,yshift=-0.20\paperheight]current page.west) {AVANS};
      
      % Institution name (small, subtle, top right area)
      \node[anchor=north east,text=accent,
            font={\sffamily\bfseries\fontsize{9}{11}\selectfont}]
        at ([xshift=-1.5cm,yshift=-0.8cm]current page.north east) {AVANS HOGESCHOOL};
      
      % Main title - centered, with breathing room
      \node[anchor=center,text=avans-black,text width=0.75\paperwidth,align=center,
            font=\rmfamily\bfseries\fontsize{38}{44}\selectfont,line width=0pt]
        at ([yshift=0.15\paperheight]current page.center) {\@title};
      
      % Subtitle if provided
      \ifx\@subtitle\@empty\else
        \node[anchor=north,text=avans-gray,text width=0.70\paperwidth,align=center,
              font=\rmfamily\fontsize{14}{17}\selectfont]
          at ([yshift=0.08\paperheight]current page.center) {\@subtitle};
      \fi
      
      % Thin separator line below title
      \draw[line width=1.2pt,color=accent] 
        ([xshift=-0.3\paperwidth,yshift=0.03\paperheight]current page.center) -- 
        ([xshift=0.3\paperwidth,yshift=0.03\paperheight]current page.center);
      
      % Programme and supervisor - centered below separator
      \node[anchor=north,text=avans-gray,text width=0.6\paperwidth,align=center,
            font=\sffamily\fontsize{11}{13}\selectfont]
        at ([yshift=-0.02\paperheight]current page.center)
        {\ifx\@programme\@empty\else {\bfseries Programme} \\ \@programme \\ [0.6em] \fi
         \ifx\@supervisor\@empty\else {\bfseries Supervisor} \\ \@supervisor \fi};
      
      % Author name - prominent, lower section
      \node[anchor=center,text=avans-black,
            font=\rmfamily\bfseries\fontsize{18}{22}\selectfont]
        at ([yshift=-0.35\paperheight]current page.center) {\@author};
      
      % Institution and date - at bottom, clean
      \node[anchor=south,text=avans-gray,text width=0.7\paperwidth,align=center,
            font=\sffamily\fontsize{10}{12}\selectfont]
        at ([yshift=1.2cm]current page.south)
        {\ifx\@institution\@empty\else \@institution \\ [0.2em] \fi
         \@date};
    \end{tikzpicture}
    
    \null
    \restoregeometry
    \pagecolor{white}
    \hypersetup{pageanchor=true}
  \end{titlepage}
}

% Figure/table caption style
\captionsetup{font=small,labelfont=bf,labelsep=period}

% Abstract style hook
\AtBeginEnvironment{abstract}{\small}

% Lists in accent color
\renewcommand*\labelitemi{\color{accent}\textbullet}
\renewcommand*\labelitemii{\color{accent}--}

\endinput
