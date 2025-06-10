import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useParams} from "react-router";

interface IdentityProvider {
    name: string;
    slug: string;
}

function Login() {
    const [identityProvider, setIdentityProvider] = useState<IdentityProvider>();
    const [error, setError] = useState<boolean>(false);
    const {t, i18n} = useTranslation();
    let {slug} = useParams();

    useEffect(() => {
        populateIdentityProviderData();
    }, []);

    if (error) {
        return <div id="container">
            <main id="main-content">
                <p>{t("backendError")}</p>
            </main>
        </div>
    }

    return (
        identityProvider !== undefined &&
        <>
            <form id="container" onSubmit={e => {
                e.preventDefault()
            }}>
                <header>
                    <h1>{identityProvider.name}</h1>
                </header>
                <main id="main-content">
                    <p>{t("loginInfo")}</p>
                </main>
                <footer>
                    <div className="actions">
                        <button id="submit-button"
                                onClick={
                                    () => location.href = `/api/identity-provider/${slug}/ui-login/${i18n.language}`
                                }
                                type="button">
                            {t("loginButton")}
                        </button>
                    </div>
                </footer>
            </form>
        </>
    );

    async function populateIdentityProviderData() {
        try {
            const response = await fetch(`/api/identity-provider/${slug}`);
            if (response.ok) {
                const data = await response.json();
                setIdentityProvider(data);
            } else {
                // Server responds with errors
                setError(true);
            }
        } catch (e) {
            // Server is unreachable
            setError(true);
        }
    }
}

export default Login;