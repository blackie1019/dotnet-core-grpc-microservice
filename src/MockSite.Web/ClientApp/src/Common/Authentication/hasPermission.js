export default function hasPermission(credential, policy) {
  if (typeof policy === 'boolean') {
    if (!policy) return true
    if (!credential) return false
    return !!credential.token && !credential.locking
  }
  if (typeof policy === 'function') return policy(credential)
  if (Array.isArray(policy)) {
    if (!credential || !credential.policies.length) return false
    const hasPolicy = policy => credential.policies.includes(policy)
    return policy.every(subPolicy =>
      Array.isArray(subPolicy)
        ? subPolicy.some(hasPolicy)
        : hasPolicy(subPolicy)
    )
  }
  return true
}
