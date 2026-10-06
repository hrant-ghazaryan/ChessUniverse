from reportlab.lib.colors import HexColor
from reportlab.lib.enums import TA_JUSTIFY
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import BaseDocTemplate, Frame, FrameBreak, PageTemplate, KeepTogether, Paragraph, Spacer, Table, TableStyle

OUTPUT = r"D:\Desktop\Hrant_Ghazaryan_CV_Armenian.pdf"
FONT = r"C:\Windows\Fonts\sylfaen.ttf"
FONT_BOLD = r"C:\Windows\Fonts\sylfaen.ttf"
pdfmetrics.registerFont(TTFont("Armenian", FONT))
pdfmetrics.registerFont(TTFont("ArmenianBold", FONT_BOLD))

base = dict(fontName="Armenian", fontSize=8.8, leading=13, textColor=HexColor("#444444"))
normal = ParagraphStyle("normal", **base)
justified = ParagraphStyle("justified", **base, alignment=TA_JUSTIFY)
small = ParagraphStyle("small", fontName="Armenian", fontSize=8.5, leading=12, textColor=HexColor("#444444"))
heading = ParagraphStyle("heading", fontName="ArmenianBold", fontSize=10, leading=13, textColor=HexColor("#222222"), spaceAfter=7)
name = ParagraphStyle("name", fontName="ArmenianBold", fontSize=22, leading=26, textColor=HexColor("#222222"))
role = ParagraphStyle("role", fontName="Armenian", fontSize=12, leading=16, textColor=HexColor("#555555"))
company = ParagraphStyle("company", fontName="ArmenianBold", fontSize=10, leading=13, textColor=HexColor("#222222"))
subtitle = ParagraphStyle("subtitle", fontName="Armenian", fontSize=8.8, leading=12, textColor=HexColor("#444444"))

def p(txt, style=normal):
    return Paragraph(txt, style)

def section(title, body, left=False):
    rule = Table([[p(title, heading)]], colWidths=[58*mm if left else 105*mm])
    rule.setStyle(TableStyle([("LINEBELOW", (0,0), (-1,-1), .45, HexColor("#e6e6e6")), ("BOTTOMPADDING", (0,0), (-1,-1), 4)]))
    return [rule, Spacer(1, 5), *body, Spacer(1, 13)]

left = []
left += section("ԿԱՊ", [p("☎   +374 96 024658", small), Spacer(1,4), p("✉   hrant2004hrant@gmail.com", small), Spacer(1,4), p("◉   github.com/hrant-ghazaryan", small), Spacer(1,4), p("in   linkedin.com/in/hrant-ghazaryan-858b93203", small)], True)
left += section("ԼԵԶՈՒՆԵՐ", [p("<b>Հայերեն</b> - Մայրենի"), p("<b>Ռուսերեն</b> - C1"), p("<b>Անգլերեն</b> - B1")], True)
left += section("ՏԵԽՆԻԿԱԿԱՆ ՀՄՏՈՒԹՅՈՒՆՆԵՐ", [p("<b>Ծրագրավորման լեզուներ</b><br/>C#, C++ (հիմնական), JavaScript (հիմնական), HTML, CSS (հիմնական)"), Spacer(1,7), p("<b>Ֆրեյմվորքեր և տեխնոլոգիաներ</b><br/>.NET, Entity Framework Core, ADO.NET, WPF<br/>ASP.NET Core Web API<br/>ASP.NET Core MVC"), Spacer(1,7), p("<b>Հիմնական գաղափարներ</b><br/>OOP, SOLID սկզբունքներ, REST API, JWT նույնականացում, LINQ"), Spacer(1,7), p("<b>Տվյալների բազաներ</b><br/>MSSQL, PostgreSQL")], True)

right = []
right += section("ՄԱՍՆԱԳԻՏԱԿԱՆ ԱՄՓՈՓՈՒՄ", [p("Մոտիվացված .NET ծրագրավորող՝ C#, ASP.NET Core և Entity Framework Core տեխնոլոգիաներով մասշտաբավորվող backend հավելվածներ մշակելու գործնական փորձով։ Մեծ ուշադրություն եմ դարձնում մաքուր, SOLID սկզբունքներին համապատասխան կոդ գրելուն և բարդ տրամաբանական խնդիրներ լուծելուն։ Թիմային աշխատանքի ուժեղ կողմեր ունեմ և պատրաստ եմ ներդրում ունենալ դինամիկ ծրագրային նախագծերում՝ շարունակաբար զարգանալով որպես ծրագրային ինժեներ։", justified)])
right += section("ԱՇԽԱՏԱՆՔԱՅԻՆ ՓՈՐՁ", [KeepTogether([p("Global IT Company", company), p(".NET ծրագրավորող | Հունվար 2023 - Օգոստոս 2023", subtitle), Spacer(1,4), p("<b>Մշակել և սպասարկել եմ RESTful API-ներ</b>՝ ASP.NET Core-ի միջոցով՝ ապահովելով frontend համակարգերի և backend-ի անխափան հաղորդակցությունը։ <b>Նախագծել և կառավարել եմ տվյալների բազայի գործողություններ</b> Entity Framework Core և SQL Server գործիքներով, ներառյալ տվյալների ստացման համար արդյունավետ LINQ հարցումների գրումը։ <b>Իրականացրել եմ backend ֆունկցիոնալ</b>՝ հետևելով օբյեկտակողմնորոշված ծրագրավորման (OOP) սկզբունքներին և մաքուր ճարտարապետության (SOLID) ուղեցույցներին՝ մասշտաբավորվող և սպասարկելի կոդ ապահովելու համար։ <b>Համագործակցել եմ մշակման թիմի հետ</b>՝ տարբերակների կառավարման համար օգտագործելով Git, մասնակցելով կոդի վերանայումներին և արդյունավետորեն լուծելով backend-ի սխալները։", justified)])])
right += section("ԿՐԹՈՒԹՅՈՒՆ", [KeepTogether([p("Microsoft Innovation Center Armenia", company), p("Full Stack .NET մշակում | Հունվար 2026 - Հուլիս 2026", subtitle)]), Spacer(1,10), KeepTogether([p("Հայաստանի ազգային պոլիտեխնիկական համալսարան", company), p("Համակարգչային գիտություն / Ծրագրային ճարտարագիտություն | 2023 - առայսօր", subtitle)]), Spacer(1,10), KeepTogether([p("Երևանի ինֆորմատիկայի պետական քոլեջ", company), p("Ծրագրավորում | 2019 - 2023", subtitle)])])

def draw_header(canvas, doc):
    canvas.saveState()
    canvas.setFont("ArmenianBold", 22)
    canvas.setFillColor(HexColor("#222222"))
    canvas.drawString(16*mm, 281*mm, "Հրանտ Ղազարյան")
    canvas.setFont("Armenian", 12)
    canvas.setFillColor(HexColor("#555555"))
    canvas.drawString(16*mm, 273*mm, ".NET ծրագրավորող")
    canvas.restoreState()

left_frame = Frame(16*mm, 13*mm, 60*mm, 250*mm, leftPadding=0, bottomPadding=0, rightPadding=3*mm, topPadding=0, id="left")
right_frame = Frame(90*mm, 13*mm, 104*mm, 250*mm, leftPadding=0, bottomPadding=0, rightPadding=0, topPadding=0, id="right")
doc = BaseDocTemplate(OUTPUT, pagesize=A4, leftMargin=0, rightMargin=0, topMargin=0, bottomMargin=0)
doc.addPageTemplates([PageTemplate(id="cv", frames=[left_frame, right_frame], onPage=draw_header)])
doc.build(left + [FrameBreak()] + right)
