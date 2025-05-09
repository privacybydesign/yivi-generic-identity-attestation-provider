import {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';
import {useParams} from "react-router"
import './App.css';
import './i18n';

interface IdentityProvider {
    name: string;
    slug: string;
}

function App() {
    const [identityProvider, setIdentityProvider] = useState<IdentityProvider>();
    const [error, setError] = useState<boolean>(false);
    const {t} = useTranslation();
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
                    <p>{t("info")}</p>
                </main>
                <footer>
                    <div className="actions">
                        <div></div>
                        <button id="submit-button" type="submit">{t("login")}</button>
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

export default App;