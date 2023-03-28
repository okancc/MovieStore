﻿using MovieStore.Models.Domain;
using MovieStore.Models.DTO;
using MovieStore.Repositories.Abstract;

namespace MovieStore.Repositories.Implementation
{


    
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext ctx;


        public MovieService(DatabaseContext ctx)
        {
            this.ctx = ctx;

        }

        public bool Add(Movie model)
        {
            try

            {
              
                ctx.Movie.Add(model);
                ctx.SaveChanges();
                foreach (int genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    ctx.MovieGenre.Add(movieGenre); 
                }
                ctx.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public bool Delete(int id)
        {
            try

            {
                var data = this.GetById(id);
                if (data==null)
                {
                    return false;
                    
                }
                var movieGenres = ctx.MovieGenre.Where(a => a.MovieId == data.Id);
                foreach (var movieGenre in movieGenres)
                {
                    ctx.MovieGenre.Remove(movieGenre);
                }
                ctx.Movie.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }


        }

        public Movie GetById(int id)
        {
            
            return ctx.Movie.Find(id);
        }

        public MovieListVm List()
        {

            var list = ctx.Movie.ToList();

            foreach (var movie in list)
            {
                var genres = (from genre in ctx.Genre join mg in ctx.MovieGenre on genre.Id equals mg.GenreId where mg.MovieId == movie.Id select genre.GenreName).ToList();
                var genreNames = string.Join(',', genres);
                movie.GenreNames = genreNames;
            }

            var data = new MovieListVm
            {
                MovieList = list.AsQueryable(),
            };

            return data;
        }

        public bool Update(Movie model)
        {
            try

            {
                var genreToDeleted = ctx.MovieGenre.Where(a => a.MovieId == model.Id && !model.Genres.Contains(a.GenreId)).ToList();

              
                foreach (var mGenre in genreToDeleted)
                {
                  
                    ctx.MovieGenre.Remove(mGenre);
                }

                foreach (int genId in model.Genres)
                {
                    var movieGenre = ctx.MovieGenre.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genId);
                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genId, MovieId = model.Id };
                        ctx.MovieGenre.Add(movieGenre);
                    }
                }


                ctx.Movie.Update(model);

             

                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public List<int> GetGenreByMovieId(int movieId)
        {
            var genreIds = ctx.MovieGenre.Where(a=> a.MovieId==movieId).Select(a => a.GenreId).ToList();
            return genreIds;
        } 
    }
}
