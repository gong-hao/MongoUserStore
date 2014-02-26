using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MongoUserStore.Models
{
    public class UserStore<TUser> :
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserStore<TUser>,
        IDisposable
        where TUser : IdentityUser
    {
        public UserStore()
        {
            this._userRepository = new UserRepository();
        }

        #region private

        private UserRepository _userRepository;

        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private void ThrowIfArgumentNull(object argument, string paramName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        #endregion private

        /*IUserLoginStore*/

        #region IUserLoginStore

        /// <summary>
        /// 將登入和使用者關聯
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            bool notExist = !user.Logins.Any(
                x =>
                    x.LoginProvider == login.LoginProvider &&
                    x.ProviderKey == login.ProviderKey
            );

            if (notExist)
            {
                user.Logins.Add(login);

                //data access
                _userRepository.UpdateUsers<TUser>(user);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// 傳回與此登入相關的使用者
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            //data access
            TUser user = _userRepository.GetUsers<TUser>()
                .Where(
                    x =>
                        x.Logins.Any(
                            y =>
                                y.LoginProvider == login.LoginProvider &&
                                y.ProviderKey == login.ProviderKey
                        )
                )
                .FirstOrDefault();

            return Task.FromResult(user);
        }

        /// <summary>
        /// 取得使用者登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            return Task.FromResult(user.Logins.ToIList());
        }

        /// <summary>
        /// 移除使用者登入
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            user.Logins
                .ToList()
                .RemoveAll(
                    x =>
                        x.LoginProvider == login.LoginProvider &&
                        x.ProviderKey == login.ProviderKey
                );

            return Task.FromResult(0);
        }

        #endregion IUserLoginStore

        /*IUserClaimStore*/

        #region IUserClaimStore

        /// <summary>
        /// 新增使用者宣告
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            ThrowIfArgumentNull(claim, "claim");

            bool notExist = !user.Claims.Any(
                x =>
                    x.ClaimType == claim.Type &&
                    x.ClaimValue == claim.Value
            );

            if (notExist)
            {
                user.Claims.Add(new UserClaim(claim.Type, claim.Value));

                //data access
                _userRepository.UpdateUsers<TUser>(user);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// 取得使用者宣告
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            IList<Claim> result = user.Claims
                .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .ToList();

            return Task.FromResult(result);
        }

        /// <summary>
        /// 移除使用者宣告
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            user.Claims.RemoveAll(x => x.ClaimType == claim.Type);

            //data access
            _userRepository.UpdateUsers<TUser>(user);

            return Task.FromResult(0);
        }

        #endregion IUserClaimStore

        /*IUserRoleStore*/

        #region IUserRoleStore

        /// <summary>
        /// 新增使用者至角色
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task AddToRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            bool notExist = !user.Roles.Contains(
                role,
                StringComparer.InvariantCultureIgnoreCase
            );

            if (notExist)
            {
                user.Roles.Add(role);

                //data access
                _userRepository.UpdateUsers<TUser>(user);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// 傳回使用者的角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            return Task.FromResult<IList<string>>(user.Roles);
        }

        /// <summary>
        /// 若使用者位於指定的角色中，則傳回 True
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            bool exist = user.Roles.Contains(
                role,
                StringComparer.InvariantCultureIgnoreCase
            );

            return Task.FromResult(exist);
        }

        /// <summary>
        /// 從角色移除使用者
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            user.Roles.RemoveAll(
                x =>
                    String.Equals(
                        x,
                        role,
                        StringComparison.InvariantCultureIgnoreCase
                    )
            );

            return Task.FromResult(0);
        }

        #endregion IUserRoleStore

        /*IUserPasswordStore*/

        #region IUserPasswordStore

        /// <summary>
        /// 取得使用者密碼雜湊
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            string passwordHash = user.PasswordHash;

            return Task.FromResult(passwordHash);
        }

        /// <summary>
        /// 若使用者有密碼集，傳回 True
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            bool hasPassword = !string.IsNullOrEmpty(user.PasswordHash);

            return Task.FromResult(hasPassword);
        }

        /// <summary>
        /// 設定使用者密碼雜湊
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        #endregion IUserPasswordStore

        /*IUserSecurityStampStore*/

        #region IUserSecurityStampStore

        /// <summary>
        /// 取得使用者安全性戳記
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// 設定使用者安全性戳記
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        #endregion IUserSecurityStampStore

        /*IUserStore*/

        #region IUserStore

        /// <summary>
        /// 插入實體
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            //data access
            _userRepository.AddUsers<TUser>(user);

            return Task.FromResult(user);
        }

        /// <summary>
        /// 標記要刪除的實體
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            //data access
            _userRepository.RemoveUsers<TUser>(user.Id);

            return Task.FromResult(true);
        }

        /// <summary>
        /// 依 ID 尋找使用者
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();

            //data access
            TUser user = _userRepository.GetUsers<TUser>()
                .Where(x => x.Id == userId)
                .FirstOrDefault();

            return Task.FromResult(user);
        }

        /// <summary>
        /// 依名稱尋找使用者
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();

            //data access
            TUser user = _userRepository.GetUsers<TUser>()
                .Where(x => x.UserName == userName)
                .FirstOrDefault();

            return Task.FromResult(user);
        }

        /// <summary>
        /// 更新實體
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();

            ThrowIfArgumentNull(user, "user");

            //data access
            _userRepository.UpdateUsers<TUser>(user);

            return Task.FromResult(user);
        }

        /// <summary>
        /// 處置存放區
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }

        #endregion IUserStore
    }
}