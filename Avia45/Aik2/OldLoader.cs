using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aik2
{
    public class OldLoader
    {
        public void LoadOld(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            if (!(MessageBox.Show("Load 3-4?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;

            ctx.Database.ExecuteSqlCommand("Delete from Pics where CraftId in (Select CraftId from Crafts where Source in ('3', '4'))");
            ctx.Database.ExecuteSqlCommand("Delete from Crafts where Source in ('3', '4')");

            var text3 = File.ReadAllText("E:\\Live\\Avia\\Text3.dat", Encoding.GetEncoding("windows-1251"));
            var text4 = File.ReadAllText("E:\\Live\\Avia\\Text4.dat", Encoding.GetEncoding("windows-1251"));

            var artId3 = ctx.Arts.Where(x => x.Mag == "Site3").Single().ArtId;
            var artId4 = ctx.Arts.Where(x => x.Mag == "Site4").Single().ArtId;

            var cnt = 0;
            var crafts = ctx.CraftsOld.Where(x => x.Source == 3 || x.Source == 4).ToList();
            foreach (var craft in crafts)
            {
                lInfo.Text = (++cnt).ToString();
                Application.DoEvents();

                var newCraft = new Crafts()
                {
                    Construct = craft.Construct,
                    Name = craft.Name,
                    Country = craft.Country,
                    IYear = craft.IYear,
                    Vert = craft.Vert,
                    Uav = craft.UAV,
                    Glider = craft.Glider,
                    Source = craft.Source.ToString(),
                    Type = craft.Type
                };
                if ((craft.TxtBeg ?? 0) > 0)
                {
                    newCraft.CText = (craft.Source == 3 ? text3 : text4).Substring(craft.TxtBeg.Value, craft.TxtEnd.Value - craft.TxtBeg.Value);
                }
                ctx.Crafts.Add(newCraft);
                ctx.SaveChanges();

                var prms = ctx.ParamsOld.Where(x => x.CraftId == craft.ID).ToList();
                if (prms.Any())
                {
                    var s = "\n\n";
                    foreach (var prm in prms)
                    {
                        s += $"\n{prm.ParamName}\t{prm.ParamData}";
                    }
                    newCraft.CText += s;
                }

                var pics = ctx.PicsOld.Where(x => x.CraftID == craft.ID).ToList();
                foreach (var pic in pics)
                {
                    var newPic = new Pics()
                    {
                        ArtId = craft.Source == 3 ? artId3 : artId4,
                        CraftId = newCraft.CraftId,
                        NNN = pic.PictID,
                        Path = pic.Picture
                    };
                    if ((pic.TxtBeg ?? 0) > 0)
                    {
                        newPic.Text = (craft.Source == 3 ? text3 : text4).Substring(pic.TxtBeg.Value, pic.TxtEnd.Value - pic.TxtBeg.Value);
                    }
                    ctx.Pics.Add(newPic);

                }
                ctx.SaveChanges();

                Util.DetachAllEntities(ctx);
            }

            lInfo.Text = "";
            MessageBox.Show("OK");
        }
    }
}
