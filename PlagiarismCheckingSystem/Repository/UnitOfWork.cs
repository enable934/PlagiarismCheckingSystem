using PlagiarismCheckingSystem.Data;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Repository;
using System;

namespace PlagiarismCheckingSystem.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly Context context;
        private GenericRepository<User> userRepository;
        private GenericRepository<LaboratoryWork> laboratoryWorkRepository;
        private GenericRepository<File> fileRepository;

        public UnitOfWork(Context context)
        {
            this.context = context;
        }

        public GenericRepository<User> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }

        public GenericRepository<LaboratoryWork> LaboratoryWorkRepository
        {
            get
            {

                if (this.laboratoryWorkRepository == null)
                {
                    this.laboratoryWorkRepository = new GenericRepository<LaboratoryWork>(context);
                }
                return laboratoryWorkRepository;
            }
        }

        public GenericRepository<File> FileRepository
        {
            get
            {

                if (this.fileRepository == null)
                {
                    this.fileRepository = new GenericRepository<File>(context);
                }
                return fileRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
