using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace Logic.Movies
{
    public class MovieMap : ClassMap<Movie>
    {
        public MovieMap()
        {
            Id(x => x.Id);

            // Discrimination is for the case - class and subclasses will be a same table in DB.
            DiscriminateSubClassesOnColumn("LicensingModel");

            Map(x => x.Name);
            //Map(x => x.LicensingModel).CustomType<int>();
            // Reveal is for non-public members mappings.
            Map(Reveal.Member<Movie>("LicensingModel")).CustomType<int>();
        }
    }

    public class TwoDaysMovieMap : SubclassMap<TwoDaysMovie>
    {
        public TwoDaysMovieMap()
        {
            // this make sure that creating instance for TwoDaysMovie class for the record containing value 1 from DB.
            DiscriminatorValue(1);
        }
    }

    public class LifeLongMovieMap : SubclassMap<LifeLongMovie>
    {
        public LifeLongMovieMap()
        {
            // this make sure that creating instance for LifeLongMovie class for the record containing value 2 from DB.
            DiscriminatorValue(2);
        }
    }
}
